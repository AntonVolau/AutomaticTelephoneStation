using AutomaticTelephoneStation.ATSManagment;
using AutomaticTelephoneStation.ClientsInfo;
using System.Collections.Generic;

namespace AutomaticTelephoneStation.ATSInfo
{
    public interface ICompany
    {
        string Name { get; }

        ICollection<IClient> Clients { get; }

        ICollection<IContract> Contracts { get; }

        IBilling Billing { get; }

        IBaseStation BaseStation { get; }
    }
}
