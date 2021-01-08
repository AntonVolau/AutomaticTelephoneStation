using AutomaticTelephoneStation.ATSEquipment;
using AutomaticTelephoneStation.ATSInfo;
using AutomaticTelephoneStation.ATSManagment;
using System;

namespace AutomaticTelephoneStation.ClientsInfo.Implementation
{
    public class Contract : IContract
    {
        public Guid ContractNumber { get; }

        public DateTime DateOfContract { get; }

        public ICompany Company { get; }

        public IPassport Passport { get; }

        public string PhoneNumber { get; }

        public ITariff Tariff { get; }

        public IEquipment Equipment { get; }

        public Contract(ICompany company, IPassport individualPassport, string phoneNumber, ITariff tariff,
            IEquipment clientEquipment)
        {
            ContractNumber = Guid.NewGuid();
            DateOfContract = DateTime.Now;
            Company = company;
            Passport = individualPassport;
            PhoneNumber = phoneNumber;
            Tariff = tariff;
            Equipment = clientEquipment;
        }
    }
}
