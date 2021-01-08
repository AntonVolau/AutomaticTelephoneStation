namespace AutomaticTelephoneStation.ClientsInfo.Implementation
{
    public class Client : IClient
    {
        public IPassport Passport { get; set; }

        public IContract Contract { get; set; }

        public string Codeword { get; set; }

        public Client(IPassport passport)
        {
            Passport = passport;
        }
    }
}