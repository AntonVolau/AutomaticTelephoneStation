namespace AutomaticTelephoneStation.ClientsInfo
{
    public interface IClient
    {
        IPassport Passport { get; }

        IContract Contract { get; set; }
    }
}
