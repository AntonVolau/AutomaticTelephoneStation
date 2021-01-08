using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSManagment.CallTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    public class BillingUnit : IBillingUnit
    {
        public ICollection<IPhone> Phones { get; }

        public ICollection<ICall> Calls { get; }

        public BillingUnit()
        {
            Phones = new Collection<IPhone>();

            Calls = new Collection<ICall>();
        }
    }
}