using AutomaticTelephoneStation.ATSInfo;
using AutomaticTelephoneStation.ATSManagment.Implementation;
using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.Enums;
using System;

namespace AutomaticTelephoneStation.ATSEquipment.Implementation
{
    public class Telephone : ITelephone
    {
        public Action<string> DisplayMethod { get; private set; }

        public event EventHandler<ConnectionEvent> ConnectedToPort;

        public event EventHandler<ConnectionEvent> DisconnectedFromPort;

        public event EventHandler<OutgoingCallEvent> NotifyPortAboutOutgoingCall;

        public event EventHandler<RejectedCallEvent> NotifyPortAboutRejectionOfCall;

        public event EventHandler<AnsweredCallEvent> NotifyPortAboutAnsweredCall;

        public Guid SerialNumber { get; }

        public TelephoneStatus TelephoneStatus { get; private set; }

        public Telephone(Action<string> displayMethod = null)
        {
            DisplayMethod = displayMethod;
            SerialNumber = Guid.NewGuid();
            TelephoneStatus = TelephoneStatus.Disabled;
        }

        public void SetDisplayMethod(Action<string> displayMethod)
        {
            DisplayMethod = displayMethod;
        }

        public void ConnectToPort(IPort port)
        {
            if (!IsPossibleToConnect(port))
            {
                DisplayMethod?.Invoke("Unable to Connect to Port");
                return;
            }

            Mapping.MergeTerminalAndPortBehaviorWhenConnecting(this, port as IPort);

            var connectionEventArgs = new ConnectionEvent(port);

            OnConnectedToPort(connectionEventArgs);

            if (connectionEventArgs.Port == null)
            {
                DisplayMethod?.Invoke("Another Telephone is Already Connected to This Port");
                return;
            }

            Mapping.ConnectTerminalToPort(this, connectionEventArgs.Port as IPort);

            TelephoneStatus = TelephoneStatus.Inaction;

            DisplayMethod?.Invoke("Connected");
        }

        private bool IsPossibleToConnect(IPort port)
        {
            return port != null && TelephoneStatus == TelephoneStatus.Disabled;
        }

        public void DisconnectFromPort()
        {
            if (TelephoneStatus == TelephoneStatus.Disabled)
            {
                DisplayMethod?.Invoke("Telephone is Already Disconnected");
                return;
            }

            var connectionEventArgs = new ConnectionEvent(null);

            OnDisconnectedFromPort(connectionEventArgs);

            Mapping.SeparateTerminalAndPortBehaviorWhenConnecting(this, connectionEventArgs.Port as IPort);

            Mapping.DisconnectTerminalFromPort(this, connectionEventArgs.Port as IPort);

            TelephoneStatus = TelephoneStatus.Disabled;

            DisplayMethod?.Invoke("Disconnected");
        }

        public void Call(string receiverPhoneNumber)
        {
            if (TelephoneStatus != TelephoneStatus.Inaction) return;

            TelephoneStatus = TelephoneStatus.OutgoingCall;

            OnNotifyPortOfOutgoingCall(new OutgoingCallEvent("", receiverPhoneNumber));
        }

        public void Answer()
        {
            if (TelephoneStatus != TelephoneStatus.IncomingCall) return;

            TelephoneStatus = TelephoneStatus.Conversation;

            DisplayMethod?.Invoke("You Answered Call");

            OnNotifyPortAboutAnsweredCall(new AnsweredCallEvent("") { CallStartTime = DateTime.Now });
        }

        public void Reject()
        {
            if (TelephoneStatus == TelephoneStatus.Inaction || TelephoneStatus == TelephoneStatus.Disabled) return;

            TelephoneStatus = TelephoneStatus.Inaction;

            DisplayMethod?.Invoke("You Rejected Call");

            OnNotifyPortAboutRejectionOfCall(new RejectedCallEvent("") { CallRejectionTime = DateTime.Now });
        }

        public void NotifyUserAboutError(object sender, FailureEvent e)
        {
            TelephoneStatus = TelephoneStatus.Inaction;

            switch (e.FailureType)
            {
                case FailureType.InsufficientFunds:
                    DisplayMethod?.Invoke("You don't have enough funds to make a call");
                    break;
                case FailureType.AbonentIsBusy:
                    DisplayMethod?.Invoke($"{e.PhoneNumber} - Abonent is Busy");
                    break;
                case FailureType.AbonentDoesNotExist:
                    DisplayMethod?.Invoke($"{e.PhoneNumber} - Abonent Doesn't Exist");
                    break;
                case FailureType.AbonentIsNotResponding:
                    DisplayMethod?.Invoke(sender is IPort port && port.PhoneNumber == e.PhoneNumber
                        ? "You Have Missed Call"
                        : $"{e.PhoneNumber} - Abonent Is Not Responding");
                    break;
                default:
                    DisplayMethod?.Invoke("Unknown Error");
                    break;
            }
        }

        public void NotifyUserAboutIncomingCall(object sender, IncomingCallEvent e)
        {
            TelephoneStatus = TelephoneStatus.IncomingCall;

            DisplayMethod?.Invoke($"{e.SenderPhoneNumber} - is calling you");
        }

        public void NotifyUserAboutRejectedCall(object sender, RejectedCallEvent e)
        {
            TelephoneStatus = TelephoneStatus.Inaction;

            DisplayMethod?.Invoke($"{e.PhoneNumberOfPersonRejectedCall} - canceled the call");
        }

        private void OnNotifyPortAboutRejectionOfCall(RejectedCallEvent e)
        {
            NotifyPortAboutRejectionOfCall?.Invoke(this, e);
        }

        private void OnNotifyPortAboutAnsweredCall(AnsweredCallEvent e)
        {
            NotifyPortAboutAnsweredCall?.Invoke(this, e);
        }

        private void OnNotifyPortOfOutgoingCall(OutgoingCallEvent e)
        {
            NotifyPortAboutOutgoingCall?.Invoke(this, e);
        }

        private void OnConnectedToPort(ConnectionEvent e)
        {
            ConnectedToPort?.Invoke(this, e);
        }

        private void OnDisconnectedFromPort(ConnectionEvent e)
        {
            DisconnectedFromPort?.Invoke(this, e);
        }
    }
}
