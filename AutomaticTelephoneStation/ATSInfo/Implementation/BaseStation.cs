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
            CancellationTime = cancellationTime; // Time in milliseconds that will be used to automatically cancell unanswerred call 
        }

        public void AddPorts(IEnumerable<IPort> ports) // nethod to attach several ports to base station
        {
            foreach (var port in ports)
            {
                AddPort(port);
            }
        }

        public void RemovePorts(IEnumerable<IPort> ports) // method to remove several ports from base station
        {
            foreach (var port in ports)
            {
                RemovePort(port);
            }
        }

        public void AddPort(IPort port)
        {
            Mapping.ConnectPortToStation(port as IPort, this); // in mapping class we can include and exclude methods to events

            Ports.Add(port); // include port to main list of ports

            Logger.WriteLine($"{port.PhoneNumber} was attached to station"); // Make log line that will inform us about phone attachment to port
        }

        public void RemovePort(IPort port)
        {
            Mapping.DisconnectPortFromStation(port as IPort, this);

            Ports.Remove(port);

            Logger.WriteLine($"{port.PhoneNumber} was disconnected from station");
        }

        public void NotifyIncomingCallPort(object sender, OutgoingCallEvent e)
        {
            var senderPort = sender as IPort;

            Logger.WriteLine($"{e.SenderPhoneNumber} is calling {e.ReceiverPhoneNumber}");

            var checkBalanceEvent = new CheckBalanceEvent(e.SenderPhoneNumber); // create an event object for check balance opperations
            OnCheckBalanceInBillingSystem(checkBalanceEvent); // Method that checks if abonent have enough balance to make a call

            Logger.WriteLine($"Checking {e.SenderPhoneNumber} balance");

            if (checkBalanceEvent.IsAllowedCall)
            {
                Logger.WriteLine($"{e.SenderPhoneNumber} has enough money to make call");

                ConnectPorts(senderPort, e);
            }
            else
            {
                Logger.WriteLine($"{e.SenderPhoneNumber} has not enough money to make call");

                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.InsufficientFunds),
                    senderPort);
            }
        }

        private void ConnectPorts(IPort senderPort, OutgoingCallEvent e)
        {
            var receiverPort = Ports.FirstOrDefault(x => x.PhoneNumber == e.ReceiverPhoneNumber); 

            if (receiverPort == null || senderPort == null || receiverPort.PortStatus == PortStatus.SwitchedOff)
            {
                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.AbonentDoesNotExist),
                    senderPort);

                Logger.WriteLine($"{e.ReceiverPhoneNumber} does not exist");
            }
            else if (receiverPort.PortStatus != PortStatus.Free)
            {
                OnNotifyPortOfFailure(new FailureEvent(e.ReceiverPhoneNumber, FailureType.AbonentIsBusy),
                    senderPort);

                Logger.WriteLine($"{e.ReceiverPhoneNumber} is busy");
            }
            else
            {
                CallsWaitingToBeAnswered.Add(senderPort, receiverPort); // add aour cakk information to specified list

                PortTimeout.Add(senderPort, SetTimer(senderPort, receiverPort)); // setting timer for call waiting to be answered, wich will end call after certain ammout of time if it wasn't answered

                OnNotifyPortOfIncomingCall(new IncomingCallEvent(senderPort.PhoneNumber), receiverPort); // This method will call the event that invokes methods that changes telephones statuses and send notifications
            }
        }

        private Timer SetTimer(IPort senderPort, IPort receiverPort)
        {
            var timer = new Timer(CancellationTime);

            timer.Elapsed += (sender, eventArgs) =>
            {
                Logger.WriteLine($"{receiverPort.PhoneNumber} Did not answered call from {senderPort.PhoneNumber}");

                OnNotifyPortOfFailure(
                    new FailureEvent(receiverPort.PhoneNumber, FailureType.AbonentIsNotResponding),
                    senderPort);

                OnNotifyPortOfFailure(
                    new FailureEvent(receiverPort.PhoneNumber, FailureType.AbonentIsNotResponding),
                    receiverPort);

                CallsWaitingToBeAnswered.Remove(senderPort);
                DisposeTimer(senderPort);

                Logger.WriteLine("Base Station Notifies Billing System about Failed Call " +
                                 $"from {senderPort.PhoneNumber} to {receiverPort.PhoneNumber}");

                OnNotifyBillingSystemAboutCallEnd(new UnansweredCallEvent(senderPort.PhoneNumber,
                    receiverPort.PhoneNumber, DateTime.Now));
            }; // creating logic wich will track time and perform certain actions if time expires

            timer.AutoReset = false; // switch off timer reset
            timer.Enabled = true; // enable set timer

            return timer;
        }

        private void DisposeTimer(IPort port)
        {
            PortTimeout[port].Dispose();
            PortTimeout.Remove(port);
        }

        public void AnswerCall(object sender, AnsweredCallEvent e)
        {
            if (!(sender is IPort receiverPort))
            {
                return;
            }

            var senderPort = CallsWaitingToBeAnswered.FirstOrDefault(x => x.Value == receiverPort).Key;

            if (senderPort == null)
            {
                return;
            }

            DisposeTimer(senderPort); // dispose timer because call was answered
            CallsWaitingToBeAnswered.Remove(senderPort); // remove information about call from this specified list

            CallsInProgress.Add(new HeldCallEvent(senderPort.PhoneNumber, receiverPort.PhoneNumber)
            { CallStartTime = e.CallStartTime }); // 

            Logger.WriteLine($"{receiverPort.PhoneNumber} answered call from {senderPort.PhoneNumber}");
        }

        public void RejectCall(object sender, RejectedCallEvent e)
        {
            if (!(sender is IPort portRejectedCall))
            {
                return;
            }

            var suitableCall = CallsInProgress.FirstOrDefault(x =>
                x.ReceiverPhoneNumber == portRejectedCall.PhoneNumber ||
                x.SenderPhoneNumber == portRejectedCall.PhoneNumber);

            var portWhichNeedToSendNotification = suitableCall is IAnsweredCall answeredCall
                ? CompleteCallInProgress(portRejectedCall, answeredCall, e)
                : CancelNotStartedCall(portRejectedCall, e); // check if call was answered and perform next actions depending on that situation

            OnNotifyPortAboutRejectionOfCall(e, portWhichNeedToSendNotification);
        }

        private IPort CompleteCallInProgress(IPort portRejectedCall, IAnsweredCall call, RejectedCallEvent e)
        {
            var portWhichNeedToSendNotification = call.SenderPhoneNumber == portRejectedCall.PhoneNumber
                ? Ports.FirstOrDefault(x => x.PhoneNumber == call.ReceiverPhoneNumber)
                : Ports.FirstOrDefault(x => x.PhoneNumber == call.SenderPhoneNumber);

            if (portWhichNeedToSendNotification != null)
            {
                Logger.WriteLine(
                $"{portRejectedCall.PhoneNumber} ended call with {portWhichNeedToSendNotification.PhoneNumber}");
            }

            CallsInProgress.Remove(call);

            Logger.WriteLine("Call completed " +
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
                portWhichNeedToSendNotification = CallsWaitingToBeAnswered.FirstOrDefault(x => x.Value == portRejectedCall).Key;

                senderPhoneNumber = portWhichNeedToSendNotification.PhoneNumber;
                receiverPhoneNumber = portRejectedCall.PhoneNumber;

                CallsWaitingToBeAnswered.Remove(portWhichNeedToSendNotification);
            }

            DisposeTimer(portRejectedCall);

            OnNotifyBillingSystemAboutCallEnd(new UnansweredCallEvent(senderPhoneNumber, receiverPhoneNumber,
                e.CallRejectionTime));

            Logger.WriteLine(
                $"{portRejectedCall.PhoneNumber} rejected call from {portWhichNeedToSendNotification.PhoneNumber}");

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
                    EventHandler<RejectedCallEvent>)?.Invoke(this, e); // changes port and telephone status and send notification
            }
        }

        private void OnNotifyBillingSystemAboutCallEnd(CallEvent e)
        {
            NotifyBillingSystemAboutCallEnd?.Invoke(this, e);
        }

        private void OnCheckBalanceInBillingSystem(CheckBalanceEvent e)
        {
            CheckBalanceInBillingSystem?.Invoke(this, e); // invokation of event that checks balance and return boolean type if call can be made or not
        }
    }
}