using AutomaticTelephoneStation.ATSManagment;

namespace AutomaticTelephoneStation.ATSEquipment.Implementation
{
    public class Phone : IPhone
    {
        public string PhoneNumber { get; }

        public ITariff Tariff { get; }

        public decimal Balance { get; private set; }

        public Phone(string phoneNumber, ITariff tariff, decimal balance = 0)
        {
            PhoneNumber = phoneNumber;
            Tariff = tariff;
            Balance = balance;
        }

        public void IncreaseBalance(decimal amountOfMoney)
        {
            Balance += amountOfMoney;
        }

        public void ReduceBalance(decimal amountOfMoney)
        {
            Balance -= amountOfMoney;
        }
    }
}