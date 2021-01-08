using System;

namespace AutomaticTelephoneStation.ClientsInfo
{
    public interface IPassport
    {
        Guid IdentificationNumber { get; }

        string FirstName { get; }

        string LastName { get; }
    }
}
