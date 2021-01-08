using System;

namespace AutomaticTelephoneStation.ATSManagment.CallTypes
{
    public interface IUnansweredCall : ICall
    {
        DateTime CallResetTime { get; set; }
    }
}