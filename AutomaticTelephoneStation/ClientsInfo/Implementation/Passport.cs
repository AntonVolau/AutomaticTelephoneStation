using System;

namespace AutomaticTelephoneStation.ClientsInfo.Implementation
{
    public class Passport : IPassport
    {
        public Guid IdentificationNumber { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public Passport(string firstName, string lastName)
        {
            IdentificationNumber = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
        }
    }
}