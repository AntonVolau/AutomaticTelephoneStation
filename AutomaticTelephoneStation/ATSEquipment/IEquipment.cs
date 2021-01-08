using AutomaticTelephoneStation.ATSInfo;

namespace AutomaticTelephoneStation.ATSEquipment
{
    public interface IEquipment
    {
        ITelephone Telephone { get; }

        IPort Port { get; }
    }
}
