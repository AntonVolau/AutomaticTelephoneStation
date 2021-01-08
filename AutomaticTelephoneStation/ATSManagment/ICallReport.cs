using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System.Collections.Generic;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface ICallReport<out TCallInfo, out TCall>
        where TCallInfo : ICallInformation<TCall>
        where TCall : ICall
    {
        IEnumerable<TCallInfo> CallInformation { get; }
    }
}