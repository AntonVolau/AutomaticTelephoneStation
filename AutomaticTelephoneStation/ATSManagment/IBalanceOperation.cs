using AutomaticTelephoneStation.CallEvents;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface IBalanceOperation
    {
        decimal GetBalance(string phoneNumber);

        void IncreaseBalance(string phoneNumber, decimal amountOfMoney);

        void ReduceBalance(string phoneNumber, decimal amountOfMoney);

        void CheckPossibilityOfCall(object sender, CheckBalanceEvent e);
    }
}