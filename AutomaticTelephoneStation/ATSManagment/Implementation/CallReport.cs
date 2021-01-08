using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    public class CallReport<TCallInfo, TCall> : ICallReport<TCallInfo, TCall>
        where TCallInfo : ICallInformation<TCall>
        where TCall : ICall
    {
        public IEnumerable<TCallInfo> CallInformation { get; }

        public CallReport(IEnumerable<TCallInfo> callInformation)
        {
            CallInformation = callInformation;
        }

        public override string ToString()
        {
            var callReportAsString = new StringBuilder();

            foreach (var callInformation in CallInformation)
            {
                callReportAsString.Append($"{callInformation}\n");
            }

            return callReportAsString.ToString();
        }
    }
}
