using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    public class ServiceClient : ChannelFactory<ISyslogServerBackupData>, ISyslogServerBackupData, IDisposable
    {
        ISyslogServerBackupData factory;


		public ServiceClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name); //ovde ce zaprav ovaj servis imati ulogu klijenta

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new SyslogCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate =
				CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}


		public void BackupLog(string message, byte[] sign)
        {

			try
			{
				factory.BackupLog(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[BackupLog] ERROR = {0}", e.Message);
			}
		}

		public void TestCommunication()
		{
			try
			{
				factory.TestCommunication();
			}
			catch (Exception e)
			{
				Console.WriteLine("[BackupLog] Didn't execute Test Communication. Error = {0}", e.Message);
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
