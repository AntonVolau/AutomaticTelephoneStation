using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System.Collections.Generic;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface IBillingUnit
    {
        ICollection<IPhone> Phones { get; }

        ICollection<ICall> Calls { get; }
    }
}