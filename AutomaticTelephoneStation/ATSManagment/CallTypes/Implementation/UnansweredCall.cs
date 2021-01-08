using System;

namespace AutomaticTelephoneStation.ATSManagment.CallTypes.Implementation
{
    public class UnansweredCall : Call, IUnansweredCall
    {
        public DateTime CallResetTime { get; set; }

        public UnansweredCall(string senderPhoneNumber, string receiverPhoneNumber, DateTime callResetTime)
            : base(senderPhoneNumber, receiverPhoneNumber)
        {
            CallResetTime = callResetTime;
        }

        public override string ToString()
        {
            return $"{base.ToString()} | {CallResetTime}";
        }
    }
}