using AutomaticTelephoneStation.ATSEquipment;

namespace AutomaticTelephoneStation.ATSManagment
{
    public interface IPhoneManagement
    {
        IPhone GetPhoneByNumber(string phoneNumber);

        void PutPhoneOnRecord(string phoneNumber, ITariff tariff);
    }
}