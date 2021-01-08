using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class RejectedCallEvent : EventArgs
    {
        public string PhoneNumberOfPersonRejectedCall { get; set; }

        public DateTime CallRejectionTime { get; set; }

        public RejectedCallEvent(string phoneNumberOfPersonRejectedCall)
        {
            PhoneNumberOfPersonRejectedCall = phoneNumberOfPersonRejectedCall;
        }
    }
}
