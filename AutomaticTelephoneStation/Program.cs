using AutomaticTelephoneStation.ATSInfo;
using AutomaticTelephoneStation.ATSInfo.Implementation;
using AutomaticTelephoneStation.ATSManagment;
using AutomaticTelephoneStation.ATSManagment.CallTypes;
using AutomaticTelephoneStation.ATSManagment.Implementation;
using AutomaticTelephoneStation.ClientsInfo.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutomaticTelephoneStation
{
    class Program
    {
        static void Main()
        {
            Action<string> displayMethod = Console.WriteLine;

            var company = new Company("ATS", new Billing(new List<ITariff> { new Tariff() }), new BaseStation());

            var tariff = company.Billing.Tariffs.First();

            var client1 = new Client(new Passport("Toha", "V"));
            var client2 = new Client(new Passport("Mark", "M"));
            var client3 = new Client(new Passport("Alexey", "S"));

            client1.Contract = company.SignContract(client1, tariff);
            client2.Contract = company.SignContract(client2, tariff);
            client3.Contract = company.SignContract(client3, tariff);

            var telephone1 = client1.Contract.Equipment.Telephone;
            var telephone2 = client2.Contract.Equipment.Telephone;
            var telephone3 = client3.Contract.Equipment.Telephone;

            var port1 = client1.Contract.Equipment.Port;
            var port2 = client2.Contract.Equipment.Port;
            var port3 = client3.Contract.Equipment.Port;

            telephone1.SetDisplayMethod(displayMethod);
            telephone2.SetDisplayMethod(displayMethod);
            telephone3.SetDisplayMethod(displayMethod);

            company.BaseStation.AddPorts(new List<IPort> { port1, port2, port3 });

            telephone1.ConnectToPort(port1);
            telephone2.ConnectToPort(port2);
            telephone3.ConnectToPort(port3);

            telephone1.Call(port2.PhoneNumber);

            telephone3.Call(port2.PhoneNumber);

            Thread.Sleep(6000);
            telephone1.Answer();
            Thread.Sleep(5000);
            telephone3.Reject();

            telephone2.Call("911");

            Console.WriteLine("Balance at 1 telephone after call: " +
                              $"{company.Billing.BalanceOperation.GetBalance(client1.Contract.PhoneNumber)}");
            Console.WriteLine("Balance at 2 telephone after call: " +
                              $"{company.Billing.BalanceOperation.GetBalance(client2.Contract.PhoneNumber)}");

            telephone1.Call(port2.PhoneNumber);

            company.Billing.BalanceOperation.IncreaseBalance(client1.Contract.PhoneNumber, 10);
            company.Billing.BalanceOperation.IncreaseBalance(client2.Contract.PhoneNumber, 10);

            telephone1.Call(port2.PhoneNumber);

            telephone2.Answer();
            Thread.Sleep(3000);
            telephone2.Reject();

            Console.WriteLine(
                company.Billing.GetCallReport<ICallInformation<IAnsweredCall>, IAnsweredCall>(port1.PhoneNumber,
                    x => x.CallCost > 0.001m && x.CallCost < 3m,
                    y => y.Duration > new TimeSpan(0, 0, 0, 1) && y.Duration < new TimeSpan(0, 0, 0, 6)));

            Console.ReadKey();
        }
    }
}
 