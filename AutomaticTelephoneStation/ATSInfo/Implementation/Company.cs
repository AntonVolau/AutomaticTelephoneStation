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
            var Equipment = new Equipment(new Telephone(), new Port(phoneNumber)); // give personal equipment to new abonent with separate port and port status set as "SwitchedOff"

            var newContract = new Contract(company, passport, phoneNumber, tariff, Equipment); // initialize new contract

            Contracts.Add(newContract); // Add new contract in company list

            AddNewClientToCompany(client); // add new client to list of clients if this client is not on the list

            OnReportBillingSystemOfNewClient(new ContractConclusionEvent(phoneNumber, tariff)); // initialize contract conclusion and invoke it

            return newContract;
        }

        private void AddNewClientToCompany(IClient client)
        {
            if (Clients.All(x => x != client)) // add new client to list of clients if this client is not on the list
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
                generatedPhoneNumber = "+37529" + Convert.ToString(random.Next(1000000, 9999999)); // generate new random phone numbers until we get phone number that is not already exist
            } 
            while (Contracts.Select(x => x.Equipment.Port.PhoneNumber).Contains(generatedPhoneNumber));

            return generatedPhoneNumber;
        }

        private void SubscribeToEvents()
        {
            ReportBillingSystemOfNewClient += Billing.PutPhoneOnRecord;
            if (!(BaseStation is IBaseStation baseStation))
            {
                return;
            }
            baseStation.NotifyBillingSystemAboutCallEnd += Billing.PutCallOnRecord;
            baseStation.CheckBalanceInBillingSystem += Billing.BalanceOperation.CheckPossibilityOfCall;
        }

        private void OnReportBillingSystemOfNewClient(ContractConclusionEvent e)
        {
            ReportBillingSystemOfNewClient?.Invoke(this, e);
        }
    }
}
