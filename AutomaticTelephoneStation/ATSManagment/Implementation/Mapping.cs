using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSInfo;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    internal static class Mapping
    {
        internal static void ConnectTelephoneToPort(ITelephone telephone, IPort port)
        {
            telephone.NotifyPortAboutOutgoingCall += port.OutgoingCall;
            port.NotifyTelephoneOfFailure += telephone.NotifyUserAboutError;
            port.NotifyTelephoneOfIncomingCall += telephone.NotifyUserAboutIncomingCall;
            telephone.NotifyPortAboutRejectionOfCall += port.RejectCall;
            port.NotifyTelephoneOfRejectionOfCall += telephone.NotifyUserAboutRejectedCall;
            telephone.NotifyPortAboutAnsweredCall += port.AnswerCall;
        }

        internal static void ConnectPortToStation(IPort port, IBaseStation baseStation)
        {
            port.NotifyStationOfOutgoingCall += baseStation.NotifyIncomingCallPort;
            baseStation.NotifyPortOfFailure += port.ReportError;
            baseStation.NotifyPortOfIncomingCall += port.IncomingCall;
            port.NotifyStationOfRejectionOfCall += baseStation.RejectCall;
            baseStation.NotifyPortOfRejectionOfCall += port.InformTelephoneAboutRejectionOfCall;
            port.NotifyStationOfAnsweredCall += baseStation.AnswerCall;
        }

        internal static void DisconnectTelephoneFromPort(ITelephone telephone, IPort port)
        {
            telephone.NotifyPortAboutOutgoingCall -= port.OutgoingCall;
            port.NotifyTelephoneOfFailure -= telephone.NotifyUserAboutError;
            port.NotifyTelephoneOfIncomingCall -= telephone.NotifyUserAboutIncomingCall;
            telephone.NotifyPortAboutRejectionOfCall -= port.RejectCall;
            port.NotifyTelephoneOfRejectionOfCall -= telephone.NotifyUserAboutRejectedCall;
            telephone.NotifyPortAboutAnsweredCall -= port.AnswerCall;
        }

        internal static void DisconnectPortFromStation(IPort port, IBaseStation baseStation)
        {
            port.NotifyStationOfOutgoingCall -= baseStation.NotifyIncomingCallPort;
            baseStation.NotifyPortOfFailure -= port.ReportError;
            baseStation.NotifyPortOfIncomingCall -= port.IncomingCall;
            port.NotifyStationOfRejectionOfCall -= baseStation.RejectCall;
            baseStation.NotifyPortOfRejectionOfCall -= port.InformTelephoneAboutRejectionOfCall;
            port.NotifyStationOfAnsweredCall -= baseStation.AnswerCall;
        }

        internal static void MergeTelephoneAndPortBehaviorWhenConnecting(ITelephone telephone, IPort port)
        {
            telephone.ConnectedToPort += port.ConnectToTelephone;
            telephone.DisconnectedFromPort += port.DisconnectFromTelephone;
        }

        internal static void SeparateTelephoneAndPortBehaviorWhenConnecting(ITelephone telephone, IPort port)
        {
            telephone.ConnectedToPort -= port.ConnectToTelephone;
            telephone.DisconnectedFromPort -= port.DisconnectFromTelephone;
        }
    }
}
