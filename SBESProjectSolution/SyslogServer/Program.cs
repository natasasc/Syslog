using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {
        //public static ServiceClient proxyBS;
        //public static X509Certificate2 certificateSign;
        public static Mutex sharedMutex = new Mutex();

        static void Main(string[] args)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfserviceb"

            string expectedSrvCertCN = "wcfbackup";
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, expectedSrvCertCN);

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                    StoreLocation.LocalMachine, signCertCN);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9988/BackupService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            //TO DO: prvo pokretanje zakomentarisati thread eventSimulator, da bi pokusali da pozovemo proxy posle ApplicationFirewalla
            //sledi komentarisanje dela gde se puni lista eventova da nam se proxy od backupa nikad ne pozove da ne bi dolazilo do pucanja konekcije
            //nakon toga otkomentarisati trehad da bi nam simuliralo eventove koji bi pristizali da bi pokazali funkcionisanje backup servera


            //var eventSimulator = new Thread(EventSimulation); //thread koji simulira da su pristigli eventi 
            //eventSimulator.Start();

            //koristicemo zajednicki mutex u ovim threadovima jer bi koristili mutex da obezbedimo pristizanje pravih eventova u bazi i slanje

            var t = new Thread(() => BackupLogOperation(binding, address, certificateSign));
            t.Start();

            //TO DO: otkomentarisati za socket error na application firewallu

            /*using (ServiceClient proxy = new ServiceClient(binding, address))
            {
                proxy.TestCommunication(); //u ovom slucaju nece vise raditi komunikacija ka application firewallu (znaci ako se prvo pozove proxy ka backup onda socket error baca tamo)
            }*/


            #region AFCC

            NetTcpBinding bindingAFCC = new NetTcpBinding();
            bindingAFCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string addressAFCC = "net.tcp://localhost:7777/SyslogServerSecurityEvent";
            ServiceHost hostAFCC = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAFCC.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), bindingAFCC, addressAFCC);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            hostAFCC.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostAFCC.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            hostAFCC.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            hostAFCC.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            hostAFCC.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostAFCC.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            ServiceSecurityAuditBehavior newAudit2 = new ServiceSecurityAuditBehavior();
            newAudit2.AuditLogLocation = AuditLogLocation.Application;
            newAudit2.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostAFCC.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostAFCC.Description.Behaviors.Add(newAudit2);

            try
            {
                hostAFCC.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }

            Console.WriteLine("AFCC process is working.");

            #endregion

            #region AF

            NetTcpBinding bindingAF = new NetTcpBinding();
            bindingAF.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string addressAF = "net.tcp://localhost:8888/SyslogServerSecurityEvent";
            ServiceHost hostAF = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAF.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), bindingAF, addressAF);

            hostAF.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            hostAF.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            // host.Credentials.ServiceCertificate.Certificate
            hostAF.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            hostAF.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostAF.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostAF.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostAF.Description.Behaviors.Add(newAudit);

            try
            {
                hostAF.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }

            Console.WriteLine("AF process is working.");

            #endregion

            #region Client

            NetTcpBinding bindingCC = new NetTcpBinding();
            string addressCC = "net.tcp://localhost:9999/SyslogServer";

            bindingCC.Security.Mode = SecurityMode.Transport;
            bindingCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingCC.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(SyslogServer));
            host.AddServiceEndpoint(typeof(ISyslogServer), bindingCC, addressCC);

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            //host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            ServiceSecurityAuditBehavior newAudit3 = new ServiceSecurityAuditBehavior();
            newAudit3.AuditLogLocation = AuditLogLocation.Application;
            newAudit3.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit3);

            host.Open();
            Console.WriteLine("Consumer Client process is working.\n");

            #endregion

            Console.ReadKey();
            host.Close();
            hostAF.Close();
            hostAFCC.Close();
        }

        public static void BackupLogOperation(NetTcpBinding binding, EndpointAddress address, X509Certificate2 certificateSign)
        {
            Console.WriteLine("Client thread processing...");
            using (ServiceClient proxy = new ServiceClient(binding, address))
            {
                //proveravace da li ima nesto u listi pristiglih eventova, ako ima kontaktira backup, salje i tamo se vrsi auditing

                while (true)
                {

                    Thread.Sleep(5000);
                    if (Database.formatedEvents.Count > 0)
                    {
                        sharedMutex.WaitOne();
                        List<string> formatedList = new List<string>(Database.formatedEvents);
                        //Console.WriteLine("Thread za slanje, broj elemenata,{0}",Database.formatedEvents.Count);
                        Database.formatedEvents.Clear();
                        Console.WriteLine("Thread za slanje, broj elemenata,{0}", formatedList.Count);
                        sharedMutex.ReleaseMutex();

                        foreach (string message in formatedList)
                        {
                            byte[] signature = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSign);

                            proxy.BackupLog(message, signature);
                            //Console.WriteLine("Backup log executed");
                        }
                    }
                }
            }
        }


        public static void EventSimulation()
        {
            Random rand = new Random();

            while (true)
            {
                Thread.Sleep(1500);
                int num = rand.Next(0, 78);
                Event ev = new Event(CriticallityLevel.GREEN_ALERT, DateTime.Now, new Consumer(String.Format("user:{0}", num), String.Format("id:{0}", num)), String.Format("Poruka:{0}", num), MessageState.OPEN);
                string message = String.Format("{0}, {1}, {2}, {3}, {4}", ev.Criticallity.ToString(), ev.Timestamp.ToString(), ev.Source.ToString(), ev.Message, ev.State.ToString());

                sharedMutex.WaitOne();
                //Console.WriteLine("Thread za kreiranje");
                Database.formatedEvents.Add(message);
                sharedMutex.ReleaseMutex();
            }
        }


    }
}
