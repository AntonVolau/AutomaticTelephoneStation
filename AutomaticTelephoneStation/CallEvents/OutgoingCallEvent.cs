using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class OutgoingCallEvent : EventArgs
    {
        public string SenderPhoneNumber { get; set; }

        public string ReceiverPhoneNumber { get; set; }

        public OutgoingCallEvent(string senderPhoneNumber, string receiverPhoneNumber)
        {
            SenderPhoneNumber = senderPhoneNumber;
            ReceiverPhoneNumber = receiverPhoneNumber;
        }
    }
}