using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.ATSManagment.CallTypes.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    public class CallManagement : ICallManagement
    {
        private IBillingUnit Data { get; }

        private IPhoneManagement PhoneManagement { get; }

        public CallManagement(IBillingUnit data, IPhoneManagement phoneManagement)
        {
            Data = data;
            PhoneManagement = phoneManagement;
        }

        public void PutCallOnRecord(ICall call)
        {
            switch (call)
            {
                case IAnsweredCall answeredCall:
                    {
                        Data.Calls.Add(new HeldCall(
                            answeredCall.SenderPhoneNumber,
                            answeredCall.ReceiverPhoneNumber,
                            answeredCall.CallStartTime,
                            answeredCall.CallEndTime));
                    }
                    break;

                case IUnansweredCall unansweredCall:
                    {
                        Data.Calls.Add(new UnansweredCall(
                            unansweredCall.SenderPhoneNumber,
                            unansweredCall.ReceiverPhoneNumber,
                            unansweredCall.CallResetTime));
                    }
                    break;
            }
        }

        public IEnumerable<T> GetCallList<T>(string phoneNumber, Func<T, bool> selector = null) where T : ICall // selector will filter our call list by including only calls that was answered
        {
            var abonentCalls = selector != null
                ? Data.Calls.OfType<T>()
                    .Where(x => x.SenderPhoneNumber == phoneNumber || x.ReceiverPhoneNumber == phoneNumber)
                    .Where(selector)
                    .ToList()
                : Data.Calls.OfType<T>()
                    .Where(x => x.SenderPhoneNumber == phoneNumber || x.ReceiverPhoneNumber == phoneNumber);

            return abonentCalls;
        }

        public decimal CalculateCostOfCall(ICall call)
        {
            if (!(call is IAnsweredCall answeredCall))
            {
                return 0; // Call costs nothing if it wasnt answered
            }

            var phone = PhoneManagement.GetPhoneByNumber(answeredCall.SenderPhoneNumber);
            var duration = answeredCall.Duration;
            var callDuration = duration.Hours * 3600 + duration.Minutes * 60;
            var callCost = phone.Tariff.PricePerMinute * callDuration;
            if (callDuration < 1)
            {
                return phone.Tariff.PricePerMinute;
            }
            else
            {
                return callCost;
            }
        }
    }
}