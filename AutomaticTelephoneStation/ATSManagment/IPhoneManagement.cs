using AutomaticTelephoneStation.ATSEquipment;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface IPhoneManagement
    {
        IPhone GetPhoneOnNumber(string phoneNumber);

        void PutPhoneOnRecord(string phoneNumber, ITariff tariff);
    }
}