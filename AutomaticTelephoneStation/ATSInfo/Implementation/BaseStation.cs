using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.ATSManagment.Implementation;
using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AutomaticTelephoneStation.ATSInfo.Implementation
{
    public class BaseStation : IBaseStation
    {
        public int CancellationTime { get; }

        public event EventHandler<IncomingCallEvent> NotifyPortOfIncomingCall;

        public event EventHandler<RejectedCallEvent> NotifyPortOfRejectionOfCall;

        public event EventHandler<FailureEvent> NotifyPortOfFailure;

        public event EventHandler<CallEvent> NotifyBillingSystemAboutCallEnd;

        public event EventHandler<CheckBalanceEvent> CheckBalanceInBillingSystem;

        public IList<IPort> Ports { get; }

        public IDictionary<IPort, IPort> CallsWaitingToBeAnswered { get; }

        public IDictionary<IPort, Timer> PortTimeout { get; }

        public IList<ICall> CallsInProgress { get; }

        public BaseStation(int cancellationTime = 4000)
        {
            CallsWaitingToBeAnswered = new Dictionary<IPort, IPort>();
            PortTimeout = new Dictionary<IPort, Timer>();
            CallsInProgress = new List<ICall>();
            Ports = new List<IPort>();
            CancellationTime = cancellationTime;
        }

        public void AddPorts(IEnumerable<IPort> ports)
        {
            foreach (var port in ports)
            {
                AddPort(port);
            }
        }

        public void RemovePorts(IEnumerable<IPort> ports)
        {
            foreach (var port in ports)
            {
                RemovePort(port);
            }
        }

        public void AddPort(IPort port)
        {
            Mapping.ConnectPortToStation(port as IPort, this);

            Ports.Add(port);

            Logger.WriteLine($"{port.PhoneNumber} was Attached to Station");
        }

        public void RemovePort(IPort port)
        {
            Mapping.DisconnectPortFromStation(port as IPort, this);

            Ports.Remove(port);

            Logger.WriteLine($"{port.PhoneNumber} was Disconnected from Station");
        }

        public void NotifyIncomingCallPort(object sender, OutgoingCallEvent e)
        {
            var senderPort = sender as IPort;

            Logger.WriteLine($"{e.SenderPhoneNumber} is Calling {e.ReceiverPhoneNumber}");

            var checkBalanceEvent = new CheckBalanceEvent(e.SenderPhoneNumber);
            OnCheckBalanceInBillingSystem(checkBalanceEvent);

            Logger.WriteLine($"Billing System Checks {e.SenderPhoneNumber} Balance");

            if (checkBalanceEvent.IsAllowedCall)
            {
                Logger.WriteLine($"{e.SenderPhoneNumber} has Enough Money to Make Call");

                ConnectPorts(senderPort, e);
            }
            else
            {
                Logger.WriteLine($"{e.SenderPhoneNumber} has not Enough Money to Make Call");

                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.InsufficientFunds),
                    senderPort);
            }
        }

        private void ConnectPorts(IPort senderPort, OutgoingCallEvent e)
        {
            var receiverPort = Ports.FirstOrDefault(x => x.PhoneNumber == e.ReceiverPhoneNumber);

            if (receiverPort == null || senderPort == null)
            {
                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.AbonentDoesNotExist),
                    senderPort);

                Logger.WriteLine($"{e.ReceiverPhoneNumber} Does not Exist");
            }
            else if (receiverPort.PortStatus != PortStatus.Free)
            {
                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.AbonentIsBusy),
                    senderPort);

                Logger.WriteLine($"{e.ReceiverPhoneNumber} is Busy");
            }
            else
            {
                CallsWaitingToBeAnswered.Add(senderPort, receiverPort);

                PortTimeout.Add(senderPort, SetTimer(senderPort, receiverPort));

                OnNotifyPortOfIncomingCall(new IncomingCallEvent(senderPort.PhoneNumber), receiverPort);
            }
        }

        private Timer SetTimer(IPort senderPort, IPort receiverPort)
        {
            var timer = new Timer(CancellationTime);

            timer.Elapsed += (sender, eventArgs) =>
            {
                Logger.WriteLine($"{receiverPort.PhoneNumber} Did not Answer Call from {senderPort.PhoneNumber}");

                OnNotifyPortOfFailure(
                    new FailureEvent(receiverPort.PhoneNumber, FailureType.AbonentIsNotResponding),
                    senderPort);

                OnNotifyPortOfFailure(
                    new FailureEvent(receiverPort.PhoneNumber, FailureType.AbonentIsNotResponding),
                    receiverPort);

                CallsWaitingToBeAnswered.Remove(senderPort);
                DisposeTimer(senderPort);

                Logger.WriteLine("Base Station Notifies Billing System of Failed Call " +
                                 $"from {senderPort.PhoneNumber} to {receiverPort.PhoneNumber}");

                OnNotifyBillingSystemAboutCallEnd(new UnansweredCallEvent(senderPort.PhoneNumber,
                    receiverPort.PhoneNumber, DateTime.Now));
            };

            timer.AutoReset = false;
            timer.Enabled = true;

            return timer;
        }

        private void DisposeTimer(IPort port)
        {
            PortTimeout[port].Dispose();
            PortTimeout.Remove(port);
        }

        public void AnswerCall(object sender, AnsweredCallEvent e)
        {
            if (!(sender is IPort receiverPort)) return;

            var senderPort = CallsWaitingToBeAnswered.FirstOrDefault(x => x.Value == receiverPort).Key;

            if (senderPort == null) return;

            DisposeTimer(senderPort);
            CallsWaitingToBeAnswered.Remove(senderPort);

            CallsInProgress.Add(new HeldCallEvent(senderPort.PhoneNumber, receiverPort.PhoneNumber)
            { CallStartTime = e.CallStartTime });

            Logger.WriteLine($"{receiverPort.PhoneNumber} Answered Call from {senderPort.PhoneNumber}");
        }

        public void RejectCall(object sender, RejectedCallEvent e)
        {
            if (!(sender is IPort portRejectedCall)) return;

            var suitableCall = CallsInProgress.FirstOrDefault(x =>
                x.ReceiverPhoneNumber == portRejectedCall.PhoneNumber ||
                x.SenderPhoneNumber == portRejectedCall.PhoneNumber);

            var portWhichNeedToSendNotification = suitableCall is IAnsweredCall answeredCall
                ? CompleteCallInProgress(portRejectedCall, answeredCall, e)
                : CancelNotStartedCall(portRejectedCall, e);

            OnNotifyPortAboutRejectionOfCall(e, portWhichNeedToSendNotification);
        }

        private IPort CompleteCallInProgress(IPort portRejectedCall, IAnsweredCall call, RejectedCallEvent e)
        {
            var portWhichNeedToSendNotification = call.SenderPhoneNumber == portRejectedCall.PhoneNumber
                ? Ports.FirstOrDefault(x => x.PhoneNumber == call.ReceiverPhoneNumber)
                : Ports.FirstOrDefault(x => x.PhoneNumber == call.SenderPhoneNumber);

            if (portWhichNeedToSendNotification != null) Logger.WriteLine(
                $"{portRejectedCall.PhoneNumber} Ended Call with {portWhichNeedToSendNotification.PhoneNumber}");

            CallsInProgress.Remove(call);

            Logger.WriteLine("Base Station Notifies Billing System of Completed call " +
                             $"from {call.SenderPhoneNumber} to {call.ReceiverPhoneNumber}");

            OnNotifyBillingSystemAboutCallEnd(new HeldCallEvent(call.SenderPhoneNumber, call.ReceiverPhoneNumber,
                call.CallStartTime, e.CallRejectionTime));

            return portWhichNeedToSendNotification;
        }

        private IPort CancelNotStartedCall(IPort portRejectedCall, RejectedCallEvent e)
        {
            IPort portWhichNeedToSendNotification;
            string senderPhoneNumber;
            string receiverPhoneNumber;

            if (CallsWaitingToBeAnswered.ContainsKey(portRejectedCall))
            {
                portWhichNeedToSendNotification = CallsWaitingToBeAnswered[portRejectedCall];

                senderPhoneNumber = portRejectedCall.PhoneNumber;
                receiverPhoneNumber = portWhichNeedToSendNotification.PhoneNumber;

                CallsWaitingToBeAnswered.Remove(portRejectedCall);
            }
            else
            {
                portWhichNeedToSendNotification =
                    CallsWaitingToBeAnswered.FirstOrDefault(x => x.Value == portRejectedCall).Key;

                senderPhoneNumber = portWhichNeedToSendNotification.PhoneNumber;
                receiverPhoneNumber = portRejectedCall.PhoneNumber;

                CallsWaitingToBeAnswered.Remove(portWhichNeedToSendNotification);
            }

            DisposeTimer(portRejectedCall);

            OnNotifyBillingSystemAboutCallEnd(new UnansweredCallEvent(senderPhoneNumber, receiverPhoneNumber,
                e.CallRejectionTime));

            Logger.WriteLine(
                $"{portRejectedCall.PhoneNumber} Rejected Call from {portWhichNeedToSendNotification.PhoneNumber}");

            return portWhichNeedToSendNotification;
        }

        private void OnNotifyPortOfIncomingCall(IncomingCallEvent e, IPort port)
        {
            if (NotifyPortOfIncomingCall?.GetInvocationList().FirstOrDefault(x => x.Target == port) != null)
            {
                (NotifyPortOfIncomingCall?.GetInvocationList().FirstOrDefault(x => x.Target == port) as
                    EventHandler<IncomingCallEvent>)?.Invoke(this, e);
            }
        }

        private void OnNotifyPortOfFailure(FailureEvent e, IPort port)
        {
            if (NotifyPortOfFailure?.GetInvocationList().FirstOrDefault(x => x.Target == port) != null)
            {
                (NotifyPortOfFailure?.GetInvocationList().First(x => x.Target == port) as
                    EventHandler<FailureEvent>)?.Invoke(this, e);
            }
        }

        private void OnNotifyPortAboutRejectionOfCall(RejectedCallEvent e, IPort port)
        {
            if (NotifyPortOfRejectionOfCall?.GetInvocationList().FirstOrDefault(x => x.Target == port) != null)
            {
                (NotifyPortOfRejectionOfCall?.GetInvocationList().First(x => x.Target == port) as
                    EventHandler<RejectedCallEvent>)?.Invoke(this, e);
            }
        }

        private void OnNotifyBillingSystemAboutCallEnd(CallEvent e)
        {
            NotifyBillingSystemAboutCallEnd?.Invoke(this, e);
        }

        private void OnCheckBalanceInBillingSystem(CheckBalanceEvent e)
        {
            CheckBalanceInBillingSystem?.Invoke(this, e);
        }
    }
}