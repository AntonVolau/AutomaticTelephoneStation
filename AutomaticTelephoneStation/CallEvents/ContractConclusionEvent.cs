using AutomaticTelephoneStation.ATSManagment;
using System;

namespace AutomaticTelephoneStation.CallEvents
{
    public class ContractConclusionEvent : EventArgs
    {
        public string PhoneNumber { get; set; }

        public ITariff Tariff { get; }

        public ContractConclusionEvent(string phoneNumber, ITariff tariff)
        {
            PhoneNumber = phoneNumber;

            Tariff = tariff;
        }
    }
}