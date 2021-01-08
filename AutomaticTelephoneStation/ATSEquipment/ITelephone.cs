using AutomaticTelephoneStation.ATSInfo;
using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.Enums;
using System;

namespace AutomaticTelephoneStation.ATSEquipment
{
    public interface ITelephone
    {
        Action<string> DisplayMethod { get; }

        Guid SerialNumber { get; }

        TelephoneStatus TelephoneStatus { get; }

        void SetDisplayMethod(Action<string> action);

        void ConnectToPort(IPort port);

        void DisconnectFromPort();

        void Call(string receiverPhoneNumber);

        void Answer();

        void Reject();

        event EventHandler<ConnectionEvent> ConnectedToPort;

        event EventHandler<ConnectionEvent> DisconnectedFromPort;

        event EventHandler<OutgoingCallEvent> NotifyPortAboutOutgoingCall;

        event EventHandler<RejectedCallEvent> NotifyPortAboutRejectionOfCall;

        event EventHandler<AnsweredCallEvent> NotifyPortAboutAnsweredCall;

        void NotifyUserAboutError(object sender, FailureEvent e);

        void NotifyUserAboutIncomingCall(object sender, IncomingCallEvent e);

        void NotifyUserAboutRejectedCall(object sender, RejectedCallEvent e);
    }
}
