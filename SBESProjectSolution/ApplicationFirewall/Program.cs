using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AFManager;
using Contracts;

namespace ApplicationFirewall
{
    class Program
    {
        public static AFCCProxy afccProxy;

        static void Main(string[] args)
        {
            string srvCertCN = "wcfserviceb";

            // Moramo da znamo gde se koji sertifikat instalira !
            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

            #region ConsumerProcess

            NetTcpBinding bindingAFCC = new NetTcpBinding();
            bindingAFCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // iscitava se server iz trusted people
            EndpointAddress addressAFCC = new EndpointAddress(new Uri("net.tcp://localhost:7777/SyslogServerSecurityEvent"),
                                      new X509CertificateEndpointIdentity(srvCert));

            afccProxy = new AFCCProxy(bindingAFCC, addressAFCC);
            
            {

                NetTcpBinding bindingCC = new NetTcpBinding();
                string addressCC = "net.tcp://localhost:5555/ClientService";

                bindingCC.Security.Mode = SecurityMode.Transport;
                bindingCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                bindingCC.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                ServiceHost serviceHost = new ServiceHost(typeof(ClientService));
                serviceHost.AddServiceEndpoint(typeof(IClientService), bindingCC, addressCC);

                serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
                serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

                ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
                newAudit.AuditLogLocation = AuditLogLocation.Application;
                newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

                serviceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
                serviceHost.Description.Behaviors.Add(newAudit);


                serviceHost.Open();

                Console.WriteLine("ClientProcess is working.");
            }
            #endregion

            #region AFSS

            NetTcpBinding bindingSS = new NetTcpBinding();
            bindingSS.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // iscitava se server iz trusted people
            EndpointAddress addressSS = new EndpointAddress(new Uri("net.tcp://localhost:8888/SyslogServerSecurityEvent"),
                                      new X509CertificateEndpointIdentity(srvCert));

            Console.WriteLine("Korisnik koji je pokrenuo ApplicationFirewall: " + WindowsIdentity.GetCurrent().Name);
            #endregion

            WhitelistConfig wc = new WhitelistConfig();
            Event ev;

            using (AFProxy proxy = new AFProxy(bindingSS, addressSS))
            {
                while(true)
                {
                    Thread.Sleep(100);
                    Console.WriteLine("Choose an option:");
                    Console.WriteLine("\t1. Add port");
                    Console.WriteLine("\t2. Remove port");
                    Console.WriteLine("\t3. Add protocol");
                    Console.WriteLine("\t4. Remove protocol");

                    string x = Console.ReadLine();
                    string input;
                    bool val;

                    switch (x)
                    {
                        case "1":
                            Console.Write("Port: ");
                            input = Console.ReadLine();
                            val = wc.AddPort(Int32.Parse(input), out ev);
                            if (!val)
                                Console.WriteLine("Port already exists.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "2":
                            Console.Write("Port: ");
                            input = Console.ReadLine();
                            val = wc.RemovePort(Int32.Parse(input), out ev);
                            if (!val)
                                Console.WriteLine("Port doesn't exist.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "3":
                            Console.Write("Protocol: ");
                            input = Console.ReadLine();
                            val = wc.AddProtocol(input, out ev);
                            if (!val)
                                Console.WriteLine("Protocol already exists.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "4":
                            Console.Write("Protocol: ");
                            input = Console.ReadLine();
                            val = wc.RemoveProtocol(input, out ev);
                            if (!val)
                                Console.WriteLine("Protocol doesn't exist.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        default:
                            Console.WriteLine("Please enter 1, 2, 3 or 4.");
                            break;
                    }
                }
            }
        }
    }
}
