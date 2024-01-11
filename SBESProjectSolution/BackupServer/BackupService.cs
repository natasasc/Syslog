using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BackupServer
{
    public class BackupService : ISyslogServerBackupData
    {
        public void BackupLog(string message, byte[] sign)
        {
            //kad je u pitanju autentifikacija putem Sertifikata
            //string clientName = "wcfserviceb";      // nije htelo onako kao na vezbama, morali smo zakucati
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string clientNameSign = clientName + "_sign"; // wcfserviceb_sign
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            /// Verify signature using SHA1 hash algorithm
            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("Sign is valid");
                try
                {
                    BackupAudit.EventSuccess(message);
                    Console.WriteLine("BackupLog executed.");
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Sign is invalid");
            }

        }

        public void TestCommunication()
        {
            Console.WriteLine("Test komunikacija izvrsena");
        }
    }
}
