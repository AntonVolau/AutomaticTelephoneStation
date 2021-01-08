using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.Enums;
using System;

namespace AutomaticTelephoneStation.ATSInfo.Implementation
{
    public class Port : IPort
    {
        public event EventHandler<OutgoingCallEvent> NotifyStationOfOutgoingCall;

        public event EventHandler<RejectedCallEvent> NotifyStationOfRejectionOfCall;

        public event EventHandler<AnsweredCallEvent> NotifyStationOfAnsweredCall;

        public event EventHandler<RejectedCallEvent> NotifyTerminalOfRejectionOfCall;

        public event EventHandler<FailureEvent> NotifyTerminalOfFailure;

        public event EventHandler<IncomingCallEvent> NotifyTerminalOfIncomingCall;

        public string PhoneNumber { get; }

        public Guid IdentificationNumber { get; }

        public PortStatus PortStatus { get; private set; }

        public Port(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            IdentificationNumber = Guid.NewGuid();
            PortStatus = PortStatus.SwitchedOff;
        }

        public void ConnectToTerminal(object sender, ConnectionEvent e)
        {
            if (PortStatus == PortStatus.SwitchedOff)
            {
                PortStatus = PortStatus.Free;
            }
            else
            {
                e.Port = null;
            }
        }

        public void DisconnectFromTerminal(object sender, ConnectionEvent e)
        {
            OnNotifyStationAboutRejectionOfCall(new RejectedCallEvent(PhoneNumber)
            { CallRejectionTime = DateTime.Now });

            PortStatus = PortStatus.SwitchedOff;
            e.Port = this;
        }

        public void OutgoingCall(object sender, OutgoingCallEvent e)
        {
            if (PortStatus != PortStatus.Free || PhoneNumber == e.ReceiverPhoneNumber) return;

            PortStatus = PortStatus.Busy;

            OnNotifyStationOfOutgoingCall(new OutgoingCallEvent(PhoneNumber, e.ReceiverPhoneNumber));
        }

        public void IncomingCall(object sender, IncomingCallEvent e)
        {
            PortStatus = PortStatus.Busy;

            OnNotifyTerminalOfIncomingCall(e);
        }

        public void AnswerCall(object sender, AnsweredCallEvent e)
        {
            OnNotifyStationOfAnsweredOfCall(new AnsweredCallEvent(PhoneNumber) { CallStartTime = e.CallStartTime });
        }

        public void RejectCall(object sender, RejectedCallEvent e)
        {
            PortStatus = PortStatus.Free;

            OnNotifyStationAboutRejectionOfCall(new RejectedCallEvent(PhoneNumber)
            { CallRejectionTime = e.CallRejectionTime });
        }

        public void InformTerminalAboutRejectionOfCall(object sender, RejectedCallEvent e)
        {
            PortStatus = PortStatus.Free;

            OnNotifyTerminalOfRejectionOfCall(e);
        }

        public void ReportError(object sender, FailureEvent e)
        {
            PortStatus = PortStatus.Free;

            OnNotifyTerminalOfFailure(e);
        }

        private void OnNotifyStationOfOutgoingCall(OutgoingCallEvent e)
        {
            NotifyStationOfOutgoingCall?.Invoke(this, e);
        }

        private void OnNotifyTerminalOfFailure(FailureEvent e)
        {
            NotifyTerminalOfFailure?.Invoke(this, e);
        }

        private void OnNotifyTerminalOfIncomingCall(IncomingCallEvent e)
        {
            NotifyTerminalOfIncomingCall?.Invoke(this, e);
        }

        private void OnNotifyStationAboutRejectionOfCall(RejectedCallEvent e)
        {
            NotifyStationOfRejectionOfCall?.Invoke(this, e);
        }

        private void OnNotifyTerminalOfRejectionOfCall(RejectedCallEvent e)
        {
            NotifyTerminalOfRejectionOfCall?.Invoke(this, e);
        }

        private void OnNotifyStationOfAnsweredOfCall(AnsweredCallEvent e)
        {
            NotifyStationOfAnsweredCall?.Invoke(this, e);
        }
    }
}
