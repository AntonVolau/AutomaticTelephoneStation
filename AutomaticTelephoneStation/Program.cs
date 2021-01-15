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
            Action<string> displayMethod = Console.WriteLine; // set delegate for standart log writing method

            var company = new Company("ATS", new Billing(new List<ITariff> { new Tariff() }), new BaseStation()); // initialize company with name, billing system and base station.

            var tariff = company.Billing.Tariffs.FirstOrDefault(); // select tariff from the list of tariffs available in company

            var client1 = new Client(new Passport("Toha", "V")); // initialize client 1
            var client2 = new Client(new Passport("Mark", "M")); // initialize client 2
            var client3 = new Client(new Passport("Alexey", "S")); // initialize client 3

            client1.Contract = company.SignContract(client1, tariff); // Contract sign performs equipment delivery and signing client to billing system for log writing purposes
            client2.Contract = company.SignContract(client2, tariff);
            client3.Contract = company.SignContract(client3, tariff);

            var telephone1 = client1.Contract.Equipment.Telephone; // initialize telephone, that 1 client owns
            var telephone2 = client2.Contract.Equipment.Telephone; // initialize telephone, that 2 client owns
            var telephone3 = client3.Contract.Equipment.Telephone; // initialize telephone, that 3 client owns

            var port1 = client1.Contract.Equipment.Port; // initialize port, that 1 client owns
            var port2 = client2.Contract.Equipment.Port; // initialize port, that 2 client owns
            var port3 = client3.Contract.Equipment.Port; // initialize port, that 3 client owns

            telephone1.SetDisplayMethod(displayMethod); // set display method to throw notifications by console messages for first telephone (by using generic Action delegate)
            telephone2.SetDisplayMethod(displayMethod); // set display method to throw notifications by console messages for second telephone (by using generic Action delegate)
            telephone3.SetDisplayMethod(displayMethod); // set display method to throw notifications by console messages for third telephone (by using generic Action delegate)

            company.BaseStation.AddPorts(new List<IPort> { port1, port2, port3 }); // initializing new list of existing ports and connect them to BaseStation

            telephone1.ConnectToPort(port1); // connect telephone of first client to the port of the same user
            telephone2.ConnectToPort(port2); // connect telephone of second client to the port of the same user
            telephone3.ConnectToPort(port3); // connect telephone of third client to the port of the same user

            telephone1.Call(port2.PhoneNumber);

            telephone3.Call(port2.PhoneNumber);

            Thread.Sleep(6000);
            telephone2.Answer();
            Thread.Sleep(5000);
            telephone2.Reject();

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
            Thread.Sleep(10000);
            telephone2.Reject();

            // company.Billing.BalanceOperation.ReduceBalance(client1.Contract.PhoneNumber, 100);

            Console.WriteLine(company.Billing.BalanceOperation.GetBalance(client1.Contract.PhoneNumber));

            telephone1.Call(port3.PhoneNumber);
            telephone3.Answer();
            Thread.Sleep(5000);
            telephone3.Reject();

            telephone3.DisconnectFromPort();

            for (int i = 1; i < 10; i++)
            {
                telephone1.Call(port2.PhoneNumber);
                telephone2.Answer();
                Thread.Sleep(i * 1000);
                telephone2.Reject();
            }

            Console.WriteLine(
                company.Billing.GetCallReport<ICallInformation<IAnsweredCall>, IAnsweredCall>(port1.PhoneNumber,
                    x => x.CallCost > 0.001m && x.CallCost < 3m,
                    y => y.Duration > new TimeSpan(0, 0, 0, 1) && y.Duration < new TimeSpan(0, 0, 1, 0))); // get log report by specific conditions

            Console.WriteLine(
                company.Billing.GetCallReport<ICallInformation<IAnsweredCall>, IAnsweredCall>(port1.PhoneNumber,
                x => x.Call.CallStartTime < DateTime.Now)); // get full call report

            Console.ReadKey();
        }
    }
}
 