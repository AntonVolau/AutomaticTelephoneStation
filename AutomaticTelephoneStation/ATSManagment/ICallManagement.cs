using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System;
using System.Collections.Generic;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface ICallManagement
    {
        void PutCallOnRecord(ICall call);

        IEnumerable<T> GetCallList<T>(string phoneNumber, Func<T, bool> selector = null) where T : ICall;

        decimal CalculateCostOfCall(ICall call);
    }
}