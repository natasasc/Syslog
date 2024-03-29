﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CertManager
    {
        /// <summary>
		/// Get a certificate with the specified subject name from the predefined certificate storage
		/// Only valid certificates should be considered
		/// </summary>
		/// <param name="storeName"></param>
		/// <param name="storeLocation"></param>
		/// <param name="subjectName"></param>
		/// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            // 4.1

            // pristupamo zadatoj lokaciji tj. storage u
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);     // samo iscitavamo sertifikat

            // samo validne sertifikate (poslednji parametar)
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);
            // po cemu pretrazujemo
            // Dobijamo kolekciju sertifikata koji su tipa CN="perinCert", MK="perinCert" ...

            foreach (X509Certificate2 cert in certCollection)   // prolazimo kroz sertifikate koji u sebi negde sadrze subjectName
            {
                // Trazimo CN
                // string cnSubjectName = $"CN={subjectName}";
                if (cert.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
                {
                    return cert;
                }
            }

            return null;
        }


        /// <summary>
        /// Get a certificate from the specified .pfx file		
        /// </summary>
        /// <param name="fileName"> .pfx file name </param>
        /// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = null;

            ///In order to create .pfx file, access to a protected .pvk file will be required.
            ///For security reasons, password must not be kept as string. .NET class SecureString provides a confidentiality of a plaintext
            //Console.Write("Insert password for the private key: ");
            //string pwd = Console.ReadLine();

            return certificate;
        }
    }
}
