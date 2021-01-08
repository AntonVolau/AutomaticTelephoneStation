using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class CallEvent : EventArgs, ICall
    {
        public string SenderPhoneNumber { get; }

        public string ReceiverPhoneNumber { get; }

        protected CallEvent(string senderPhoneNumber, string receiverPhoneNumber)
        {
            SenderPhoneNumber = senderPhoneNumber;

            ReceiverPhoneNumber = receiverPhoneNumber;
        }
    }
}
