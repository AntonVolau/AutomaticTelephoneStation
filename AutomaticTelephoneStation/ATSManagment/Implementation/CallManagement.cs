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

        public IEnumerable<T> GetCallList<T>(string phoneNumber, Func<T, bool> selector = null) where T : ICall
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
            if (!(call is IAnsweredCall answeredCall)) return 0;

            var phone = PhoneManagement.GetPhoneOnNumber(answeredCall.SenderPhoneNumber);
            var duration = answeredCall.Duration;
            var callDurationInSeconds = duration.Hours * 3600 + duration.Minutes * 60 + duration.Seconds;
            var pricePerSecond = phone.Tariff.PricePerMinute / 60;
            var callCost = callDurationInSeconds * pricePerSecond;

            return callCost;
        }
    }
}