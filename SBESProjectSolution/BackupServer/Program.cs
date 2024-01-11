using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace BackupServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfbackup"

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9988/BackupService";
            ServiceHost hostBS = new ServiceHost(typeof(BackupService));
            hostBS.AddServiceEndpoint(typeof(ISyslogServerBackupData), binding, address);

            hostBS.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostBS.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
			hostBS.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostBS.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new BackupCertValidator();
            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
			hostBS.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            hostBS.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            //SERVERSKI SERTIFIKAT TREBA DA BUDE IZDAT OD STRANE SYSLOG_CA A POSTO ZA AF TREBA DA BUDE ISTI KAO I SERVERSKI ONDA BI I ON TREBA OD NJEGA DA BUDE IZDAT

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostBS.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostBS.Description.Behaviors.Add(newAudit);


            try
            {
                hostBS.Open();
                Console.WriteLine("Service for backing up data is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
        }
    }
}
