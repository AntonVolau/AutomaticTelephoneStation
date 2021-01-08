using System;

namespace AutomaticTelephoneStation.ATSManagment.CallTypes
{
    public interface IAnsweredCall : ICall
    {
        DateTime CallStartTime { get; set; }

        DateTime CallEndTime { get; set; }

        TimeSpan Duration { get; }
    }
}