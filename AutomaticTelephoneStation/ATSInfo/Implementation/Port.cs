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

        public event EventHandler<RejectedCallEvent> NotifyTelephoneOfRejectionOfCall;

        public event EventHandler<FailureEvent> NotifyTelephoneOfFailure;

        public event EventHandler<IncomingCallEvent> NotifyTelephoneOfIncomingCall;

        public string PhoneNumber { get; }

        public Guid IdentificationNumber { get; }

        public PortStatus PortStatus { get; private set; }

        public Port(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            IdentificationNumber = Guid.NewGuid();
            PortStatus = PortStatus.SwitchedOff;
        }

        public void ConnectToTelephone(object sender, ConnectionEvent e)
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

        public void DisconnectFromTelephone(object sender, ConnectionEvent e)
        {
            OnNotifyStationAboutRejectionOfCall(new RejectedCallEvent(PhoneNumber)
            { CallRejectionTime = DateTime.Now });

            PortStatus = PortStatus.SwitchedOff;
            e.Port = this;
        }

        public void OutgoingCall(object sender, OutgoingCallEvent e)
        {
            if (PortStatus != PortStatus.Free || PhoneNumber == e.ReceiverPhoneNumber)
            {
                return;
            }

            PortStatus = PortStatus.Busy;

            OnNotifyStationOfOutgoingCall(new OutgoingCallEvent(PhoneNumber, e.ReceiverPhoneNumber));
        }

        public void IncomingCall(object sender, IncomingCallEvent e)
        {
            PortStatus = PortStatus.Busy;

            OnNotifyTelephoneOfIncomingCall(e); 
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

        public void InformTelephoneAboutRejectionOfCall(object sender, RejectedCallEvent e)
        {
            PortStatus = PortStatus.Free;

            OnNotifyTelephoneOfRejectionOfCall(e);
        }

        public void ReportError(object sender, FailureEvent e)
        {
            PortStatus = PortStatus.Free;

            OnNotifyTelephoneOfFailure(e);
        }

        private void OnNotifyStationOfOutgoingCall(OutgoingCallEvent e)
        {
            NotifyStationOfOutgoingCall?.Invoke(this, e);
        }

        private void OnNotifyTelephoneOfFailure(FailureEvent e)
        {
            NotifyTelephoneOfFailure?.Invoke(this, e);
        }

        private void OnNotifyTelephoneOfIncomingCall(IncomingCallEvent e)
        {
            NotifyTelephoneOfIncomingCall?.Invoke(this, e);
        }

        private void OnNotifyStationAboutRejectionOfCall(RejectedCallEvent e)
        {
            NotifyStationOfRejectionOfCall?.Invoke(this, e);
        }

        private void OnNotifyTelephoneOfRejectionOfCall(RejectedCallEvent e)
        {
            NotifyTelephoneOfRejectionOfCall?.Invoke(this, e);
        }

        private void OnNotifyStationOfAnsweredOfCall(AnsweredCallEvent e)
        {
            NotifyStationOfAnsweredCall?.Invoke(this, e);
        }
    }
}
