using AutomaticTelephoneStation.ATSEquipment.Implementation;
using AutomaticTelephoneStation.ATSManagment;
using AutomaticTelephoneStation.CallEvents;
using AutomaticTelephoneStation.ClientsInfo;
using AutomaticTelephoneStation.ClientsInfo.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomaticTelephoneStation.ATSInfo.Implementation
{
    public class Company : ICompany
    {
        public event EventHandler<ContractConclusionEvent> ReportBillingSystemOfNewClient;

        public string Name { get; }

        public ICollection<IClient> Clients { get; }

        public ICollection<IContract> Contracts { get; }

        public IBilling Billing { get; }

        public IBaseStation BaseStation { get; }

        public Company(string name, IBilling billing, IBaseStation baseStation)
        {
            Name = name;
            Clients = new List<IClient>();
            Contracts = new List<IContract>();
            Billing = billing;
            BaseStation = baseStation;

            SubscribeToEvents();
        }

        public IContract SignContract(IClient client, ITariff selectedTariff)
        {
            var company = this;
            var passport = client.Passport;
            var phoneNumber = GetUniquePhoneNumber();
            var tariff = selectedTariff;
            var Equipment = new Equipment(new Telephone(), new Port(phoneNumber));

            var newContract = new Contract(passport, phoneNumber, tariff, Equipment);

            Contracts.Add(newContract);

            AddNewClientToCompany(client);

            OnReportBillingSystemOfNewClient(new ContractConclusionEvent(phoneNumber, tariff));

            return newContract;
        }

        private void AddNewClientToCompany(IClient client)
        {
            if (Clients.All(x => x != client))
            {
                Clients.Add(client);
            }
        }

        private string GetUniquePhoneNumber()
        {
            var random = new Random();

            string generatedPhoneNumber;

            do
            {
                generatedPhoneNumber = Convert.ToString(random.Next());
            } while (Contracts.Select(x => x.Equipment.Port.PhoneNumber).Contains(generatedPhoneNumber));

            return generatedPhoneNumber;
        }

        private void SubscribeToEvents()
        {
            ReportBillingSystemOfNewClient += Billing.PutPhoneOnRecord;
            if (!(BaseStation is IBaseStation baseStation)) return;
            baseStation.NotifyBillingSystemAboutCallEnd += Billing.PutCallOnRecord;
            baseStation.CheckBalanceInBillingSystem += Billing.BalanceOperation.CheckPossibilityOfCall;
        }

        private void OnReportBillingSystemOfNewClient(ContractConclusionEvent e)
        {
            ReportBillingSystemOfNewClient?.Invoke(this, e);
        }
    }
}
