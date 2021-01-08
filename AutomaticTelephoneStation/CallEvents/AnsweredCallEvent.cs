using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class AnsweredCallEvent : EventArgs
    {
        public string PhoneNumberOfPersonAnsweredCall { get; set; }

        public DateTime CallStartTime { get; set; }

        public AnsweredCallEvent(string phoneNumberOfPersonAnsweredCall)
        {
            PhoneNumberOfPersonAnsweredCall = phoneNumberOfPersonAnsweredCall;
        }
    }
}
