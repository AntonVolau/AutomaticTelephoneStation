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

        public IPhone GetPhoneByNumber(string phoneNumber)
        {
            return Data.Phones.FirstOrDefault(x => x.PhoneNumber == phoneNumber) ?? // return phone number that satisfy the condition ot throw exception if there are non
                   throw new Exception("Phone number doesn't exist");
        }

        public void PutPhoneOnRecord(string phoneNumber, ITariff tariff)
        {
            Data.Phones.Add(new Phone(phoneNumber, tariff)); // Data represent a collection of phones and collection of calls. This method includes new phone for tracking its events and keeping log lines 
        }
    }
}