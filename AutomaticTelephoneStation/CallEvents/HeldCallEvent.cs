using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class HeldCallEvent : CallEvent, IAnsweredCall
    {
        public DateTime CallStartTime { get; set; }

        public DateTime CallEndTime { get; set; }

        public TimeSpan Duration => CallEndTime - CallStartTime;

        public HeldCallEvent(string senderPhoneNumber, string receiverPhoneNumber)
            : base(senderPhoneNumber, receiverPhoneNumber)
        {
        }

        public HeldCallEvent(string senderPhoneNumber, string receiverPhoneNumber, DateTime callStartTime, DateTime callEndTime)
            : base(senderPhoneNumber, receiverPhoneNumber)
        {
            CallStartTime = callStartTime;

            CallEndTime = callEndTime;
        }
    }
}
