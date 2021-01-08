using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSInfo;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    internal static class Mapping
    {
        internal static void ConnectTerminalToPort(ITelephone telephone, IPort port)
        {
            telephone.NotifyPortAboutOutgoingCall += port.OutgoingCall;
            port.NotifyTerminalOfFailure += telephone.NotifyUserAboutError;
            port.NotifyTerminalOfIncomingCall += telephone.NotifyUserAboutIncomingCall;
            telephone.NotifyPortAboutRejectionOfCall += port.RejectCall;
            port.NotifyTerminalOfRejectionOfCall += telephone.NotifyUserAboutRejectedCall;
            telephone.NotifyPortAboutAnsweredCall += port.AnswerCall;
        }

        internal static void ConnectPortToStation(IPort port, IBaseStation baseStation)
        {
            port.NotifyStationOfOutgoingCall += baseStation.NotifyIncomingCallPort;
            baseStation.NotifyPortOfFailure += port.ReportError;
            baseStation.NotifyPortOfIncomingCall += port.IncomingCall;
            port.NotifyStationOfRejectionOfCall += baseStation.RejectCall;
            baseStation.NotifyPortOfRejectionOfCall += port.InformTerminalAboutRejectionOfCall;
            port.NotifyStationOfAnsweredCall += baseStation.AnswerCall;
        }

        internal static void DisconnectTerminalFromPort(ITelephone telephone, IPort port)
        {
            telephone.NotifyPortAboutOutgoingCall -= port.OutgoingCall;
            port.NotifyTerminalOfFailure -= telephone.NotifyUserAboutError;
            port.NotifyTerminalOfIncomingCall -= telephone.NotifyUserAboutIncomingCall;
            telephone.NotifyPortAboutRejectionOfCall -= port.RejectCall;
            port.NotifyTerminalOfRejectionOfCall -= telephone.NotifyUserAboutRejectedCall;
            telephone.NotifyPortAboutAnsweredCall -= port.AnswerCall;
        }

        internal static void DisconnectPortFromStation(IPort port, IBaseStation baseStation)
        {
            port.NotifyStationOfOutgoingCall -= baseStation.NotifyIncomingCallPort;
            baseStation.NotifyPortOfFailure -= port.ReportError;
            baseStation.NotifyPortOfIncomingCall -= port.IncomingCall;
            port.NotifyStationOfRejectionOfCall -= baseStation.RejectCall;
            baseStation.NotifyPortOfRejectionOfCall -= port.InformTerminalAboutRejectionOfCall;
            port.NotifyStationOfAnsweredCall -= baseStation.AnswerCall;
        }

        internal static void MergeTerminalAndPortBehaviorWhenConnecting(ITelephone telephone, IPort port)
        {
            telephone.ConnectedToPort += port.ConnectToTerminal;
            telephone.DisconnectedFromPort += port.DisconnectFromTerminal;
        }

        internal static void SeparateTerminalAndPortBehaviorWhenConnecting(ITelephone telephone, IPort port)
        {
            telephone.ConnectedToPort -= port.ConnectToTerminal;
            telephone.DisconnectedFromPort -= port.DisconnectFromTerminal;
        }
    }
}
