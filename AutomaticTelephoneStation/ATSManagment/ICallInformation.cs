using AutomaticTelephoneStation.ATSManagment.CallTypes;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface ICallInformation<out T> where T : ICall
    {
        T Call { get; }

        decimal CallCost { get; }
    }
}