namespace AutomaticTelephoneStation.ATSManagment.CallTypes
{
    public interface ICall
    {
        string SenderPhoneNumber { get; }

        string ReceiverPhoneNumber { get; }
    }
}
