using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.Enums;
using System;

namespace AutomaticTelephoneStation.ATSInfo
{
    public interface IPort
    {
        string PhoneNumber { get; }

        Guid IdentificationNumber { get; }

        PortStatus PortStatus { get; }

        event EventHandler<OutgoingCallEvent> NotifyStationOfOutgoingCall;

        event EventHandler<RejectedCallEvent> NotifyStationOfRejectionOfCall;

        event EventHandler<AnsweredCallEvent> NotifyStationOfAnsweredCall;

        event EventHandler<RejectedCallEvent> NotifyTelephoneOfRejectionOfCall;

        event EventHandler<FailureEvent> NotifyTelephoneOfFailure;

        event EventHandler<IncomingCallEvent> NotifyTelephoneOfIncomingCall;

        void ConnectToTelephone(object sender, ConnectionEvent e);

        void DisconnectFromTelephone(object sender, ConnectionEvent e);

        void OutgoingCall(object sender, OutgoingCallEvent e);

        void IncomingCall(object sender, IncomingCallEvent e);

        void AnswerCall(object sender, AnsweredCallEvent e);

        void RejectCall(object sender, RejectedCallEvent e);

        void InformTelephoneAboutRejectionOfCall(object sender, RejectedCallEvent e);

        void ReportError(object sender, FailureEvent e);
    }
}
