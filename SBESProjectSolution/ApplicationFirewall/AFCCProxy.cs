using AFManager;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationFirewall
{
    public class AFCCProxy : ChannelFactory<ISyslogServerSecurityEvent>, ISyslogServerSecurityEvent, IDisposable
    {
        ISyslogServerSecurityEvent factory;

        public AFCCProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            // 1
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfclient"
            //cltCertCN = cltCertCN.ToLower();

            // 2
            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            // U prvom zadatku ChainTrust, u drugom Custom
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();

            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // 1
            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            // klijentska operacija trazi klijentov sertifikat => StoreName.My (my computer tj. Personal)
            // cuvanje sertifikata tamo gde treba

            factory = this.CreateChannel();
        }

        public void sendEvent(Event ev)
        {
            try
            {
                factory.sendEvent(ev);
            }
            catch (Exception e)
            {
                Console.WriteLine("[sendEvent] ERROR = {0}", e.Message);
            }
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
    }
}
