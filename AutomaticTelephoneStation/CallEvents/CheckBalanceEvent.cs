namespace AutomaticTelephoneStation.CallEvents
{
    public class CheckBalanceEvent
    {
        public string PhoneNumber { get; set; }

        public bool IsAllowedCall { get; set; }

        public CheckBalanceEvent(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}
