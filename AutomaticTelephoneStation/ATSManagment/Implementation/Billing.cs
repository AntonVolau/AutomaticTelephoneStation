using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.CallEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    public class Billing : IBilling
    {
        public IEnumerable<ITariff> Tariffs { get; }

        public IBalanceOperation BalanceOperation { get; }

        private IBillingUnit Data { get; }

        private IPhoneManagement PhoneManagement { get; }

        private ICallManagement CallManagement { get; }

        public Billing(IEnumerable<ITariff> tariffs)
        {
            Data = new BillingUnit();
            PhoneManagement = new PhoneManagement(Data);
            BalanceOperation = new BalanceOperation(PhoneManagement);
            CallManagement = new CallManagement(Data, PhoneManagement);
            Tariffs = tariffs;
        }

        public void PutCallOnRecord(object sender, ICall call)
        {
            CallManagement.PutCallOnRecord(call);

            BalanceOperation.ReduceBalance(call.SenderPhoneNumber, CallManagement.CalculateCostOfCall(call));
        }

        public void PutPhoneOnRecord(object sender, ContractConclusionEvent e)
        {
            PhoneManagement.PutPhoneOnRecord(e.PhoneNumber, e.Tariff);
        }

        public ICallReport<TCallInfo, TCall> GetCallReport<TCallInfo, TCall>(string phoneNumber,
            Func<TCallInfo, bool> selectorCallInfo = null, Func<TCall, bool> selectorCall = null) // Func delegates will filter list of calls by certain conditions
            where TCallInfo : ICallInformation<TCall>
            where TCall : ICall
        {
            var abonentCalls = CallManagement.GetCallList(phoneNumber, selectorCall);

            var callInformationList = selectorCallInfo != null
                ? abonentCalls.Select(call =>
                        new CallInformation<TCall>(call, CallManagement.CalculateCostOfCall(call)))
                    .OfType<TCallInfo>()
                    .Where(selectorCallInfo)
                    .ToList()
                : abonentCalls.Select(call =>
                        new CallInformation<TCall>(call, CallManagement.CalculateCostOfCall(call)))
                    .OfType<TCallInfo>()
                    .ToList();

            return new CallReport<TCallInfo, TCall>(callInformationList);
        }
    }
}
