using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSEquipment.Implementation;
using System;
using System.Linq;

namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    internal class PhoneManagement : IPhoneManagement
    {
        private IBillingUnit Data { get; }

        public PhoneManagement(IBillingUnit data)
        {
            Data = data;
        }

        public IPhone GetPhoneOnNumber(string phoneNumber)
        {
            return Data.Phones.FirstOrDefault(x => x.PhoneNumber == phoneNumber) ??
                   throw new Exception("Phone number doesn't exist");
        }

        public void PutPhoneOnRecord(string phoneNumber, ITariff tariff)
        {
            Data.Phones.Add(new Phone(phoneNumber, tariff));
        }
    }
}