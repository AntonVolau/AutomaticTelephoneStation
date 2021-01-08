using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class IncomingCallEvent : EventArgs
    {
        public string SenderPhoneNumber { get; set; }

        public IncomingCallEvent(string senderPhoneNumber)
        {
            SenderPhoneNumber = senderPhoneNumber;
        }
    }
}
