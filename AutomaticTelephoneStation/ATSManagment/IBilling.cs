using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.CallEvents;
using System;
using System.Collections.Generic;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface IBilling
    {
        IEnumerable<ITariff> Tariffs { get; }

        IBalanceOperation BalanceOperation { get; }

        ICallReport<TCallInfo, TCall> GetCallReport<TCallInfo, TCall>(string phoneNumber,
            Func<TCallInfo, bool> selectorCallInfo = null, Func<TCall, bool> selectorCall = null)
            where TCallInfo : ICallInformation<TCall>
            where TCall : ICall;

        void PutCallOnRecord(object sender, ICall e);

        void PutPhoneOnRecord(object sender, ContractConclusionEvent e);
    }
}
