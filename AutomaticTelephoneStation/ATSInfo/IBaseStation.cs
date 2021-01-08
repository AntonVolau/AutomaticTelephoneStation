using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.CallEvents;
using System;
using System.Collections.Generic;
using System.Timers;

namespace AutomaticTelephoneStation.ATSInfo
{
    public interface IBaseStation
    {
        int CancellationTime { get; }

        IList<IPort> Ports { get; }

        IDictionary<IPort, IPort> CallsWaitingToBeAnswered { get; }

        IDictionary<IPort, Timer> PortTimeout { get; }

        IList<ICall> CallsInProgress { get; }

        void AddPorts(IEnumerable<IPort> ports);

        void RemovePorts(IEnumerable<IPort> ports);

        void AddPort(IPort port);

        void RemovePort(IPort port);

        void NotifyIncomingCallPort(object sender, OutgoingCallEvent e);

        void AnswerCall(object sender, AnsweredCallEvent e);

        void RejectCall(object sender, RejectedCallEvent e);

        event EventHandler<IncomingCallEvent> NotifyPortOfIncomingCall;

        event EventHandler<RejectedCallEvent> NotifyPortOfRejectionOfCall;

        event EventHandler<FailureEvent> NotifyPortOfFailure;

        event EventHandler<CallEvent> NotifyBillingSystemAboutCallEnd;

        event EventHandler<CheckBalanceEvent> CheckBalanceInBillingSystem;
    }
}
