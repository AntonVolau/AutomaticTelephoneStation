using AutomaticTelephoneStation.ATSManagment;

namespace AutomaticTelephoneStation.ATSEquipment
{
    public interface IPhone
    {
        string PhoneNumber { get; }

        ITariff Tariff { get; }

        decimal Balance { get; }

        void IncreaseBalance(decimal amountOfMoney);

        void ReduceBalance(decimal amountOfMoney);
    }
}