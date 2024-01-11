using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the service side.
        /// Service should consider certificate valid if its issuer is the same as the issuer of the service.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)     // klijentov sertifikat
        {
            // KLIJENTSKI CERT JE VALIDAN AKO JE IZDAT U PRETHODNIH MESEC DANA
            DateTime datum = DateTime.Now;
            int mesec = datum.Month;

            DateTime datum2;
            if (mesec != 1)
            {
                if (mesec == 3 && datum.Day > 28)
                {
                    if (datum.Year % 4 == 0)
                        datum2 = new DateTime(datum.Year, mesec - 1, 29);
                    else
                        datum2 = new DateTime(datum.Year, mesec - 1, 28);
                }
                else if ((mesec == 5 || mesec == 7 || mesec == 10 || mesec == 12) && datum.Day == 31)
                    datum2 = new DateTime(datum.Year, mesec - 1, 30);
                else
                    datum2 = new DateTime(datum.Year, mesec - 1, datum.Day);
            }
            else
            {
                datum2 = new DateTime(datum.Year - 1, 12, datum.Day);
            }

            if (certificate.NotBefore < datum2)
            {
                throw new Exception("Certificate is not valid. (Too old)");
            }

            // dobavljamo sertifikat servisa iz storage a
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine,
                Formatter.ParseName(WindowsIdentity.GetCurrent().Name));

            if (!certificate.Issuer.Equals(srvCert.Issuer))     // nisu izdati od istog sertifikacionog tela
            {
                throw new Exception("Certificate is not from the valid issuer.");
            }
        }
    }
}
