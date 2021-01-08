using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class UnansweredCallEvent : CallEvent, IUnansweredCall
    {
        public DateTime CallResetTime { get; set; }

        public UnansweredCallEvent(string senderPhoneNumber, string receiverPhoneNumber, DateTime callResetTime)
            : base(senderPhoneNumber, receiverPhoneNumber)
        {
            CallResetTime = callResetTime;
        }
    }
}
