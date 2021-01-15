namespace AutomaticTelephoneStation.ATSManagment.Implementation
{
    public class Tariff : ITariff
    {
        public string TariffName { get; } = "Unlimited";
        public decimal PricePerMinute { get; } = 0.05m;
    }
}
