using AutomaticTelephoneStation.Enums;
using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class FailureEvent : EventArgs
    {
        public string PhoneNumber { get; set; }

        public FailureType FailureType { get; set; }

        public FailureEvent(string phoneNumber, FailureType failureType)
        {
            PhoneNumber = phoneNumber;
            FailureType = failureType;
        }
    }
}
