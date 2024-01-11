using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class SyslogCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (!certificate.Issuer.Equals("CN=SYSLOG_CA"))
                throw new Exception("Certificate isn't issued by SYSLOG_CA");

            DateTime threeYears = DateTime.Now;
            threeYears.AddYears(3);
            if (DateTime.Compare(certificate.NotAfter, threeYears) < 0) // Compare(d1,d1) ... < 0 -> d1 is earlier than d2
                throw new Exception("Certificate is valid less than 3 years from the moment of authentification");
        }
    }
}
