using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;

namespace ConsumerClient
{
    class Program
    {
        static void Main(string[] args)
        {

            #region AF

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:5555/ClientService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);
            // korisnik koji je pokrenuo app

            string protocol = "";
            string port = "";

            Consumer c = new Consumer(ParseName(WindowsIdentity.GetCurrent().Name), WindowsIdentity.GetCurrent().User.ToString());

            while (true)
            {
                Console.Write("Protocol: ");
                protocol = Console.ReadLine();
                Console.Write("Port: ");
                port = Console.ReadLine();

                using (CAFProxy proxy = new CAFProxy(binding, new EndpointAddress(new Uri(address))))
                {

                    if (proxy.CheckPP(protocol, port, c))
                        break;
                    else
                        Console.WriteLine("Protocol or port is not on whitelist.");
                }
            }

            #endregion

            #region SyslogServer

            NetTcpBinding binding2 = new NetTcpBinding();
            string address2 = "net.tcp://localhost:9999/SyslogServer";

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            #endregion

            using (ConsumerClient proxy = new ConsumerClient(binding2, new EndpointAddress(new Uri(address2))))
            {

                while (true)
                {
                    PrintMenu();
                    string x = Console.ReadLine();

                    switch (x)
                    {
                        case "1":
                            proxy.Read();
                            break;
                        case "2":
                            proxy.Update(0,0);
                            break;
                        case "3":
                            proxy.Delete(0);
                            break;
                        case "4":
                            proxy.Subscribe();
                            break;
                    }
                }
            }
        }


        static void PrintMenu()
        {
            Console.WriteLine("Odaberite opciju:");
            Console.WriteLine("1. View new recieved event logs");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Subscribe");
        }


        public static string ParseName(string winLogonName)
        {
            string[] parts = new string[] { };

            if (winLogonName.Contains("@"))
            {
                ///UPN format
                parts = winLogonName.Split('@');
                return parts[0];
            }
            else if (winLogonName.Contains("\\"))
            {
                /// SPN format
                parts = winLogonName.Split('\\');
                return parts[1];
            }
            else if (winLogonName.Contains("CN"))
            {
                // sertifikati, name je formiran kao CN=imeKorisnika;
                int startIndex = winLogonName.IndexOf("=") + 1;
                int endIndex = winLogonName.IndexOf(";");
                string s = winLogonName.Substring(startIndex, endIndex - startIndex);
                return s;
            }
            else
            {
                return winLogonName;
            }
        }
    }
}
