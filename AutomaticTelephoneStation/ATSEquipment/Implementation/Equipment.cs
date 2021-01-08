using AutomaticTelephoneStation.ATSInfo;

namespace AutomaticTelephoneStation.ATSEquipment.Implementation
{
    public class Equipment : IEquipment
    {
        public ITelephone Telephone { get; }

        public IPort Port { get; }

        public Equipment(ITelephone telephone, IPort port)
        {
            Telephone = telephone;
            Port = port;
        }
    }
}
