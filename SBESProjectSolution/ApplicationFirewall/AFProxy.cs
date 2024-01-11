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
    public class AFProxy : ChannelFactory<ISyslogServerSecurityEvent>, ISyslogServerSecurityEvent, IDisposable
    {
        ISyslogServerSecurityEvent factory;

        public AFProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfaf"
            //cltCertCN = cltCertCN.ToLower();

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode 
                = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;

            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
            // klijentska operacija trazi klijentov sertifikat => StoreName.My (my computer tj. Personal)

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
                Console.WriteLine("Error: {0}", e.Message);
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
