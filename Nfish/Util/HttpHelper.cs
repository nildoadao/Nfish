using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Nfish.Util
{
    class HttpHelper
    {
        private static HttpClient client;

        private static Version version = new AssemblyName(typeof(HttpHelper).Assembly.FullName).Version;
        
        public static HttpClient Create()
        {
            if(client == null)
            {
                ServicePointManager.ServerCertificateValidationCallback += IgnoreCertificates;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;                
                client = new HttpClient();               
                client.DefaultRequestHeaders.Add("User-Agent", string.Format("Nfish {0}", version.ToString()));
            }
            return client;
        }

        private static bool IgnoreCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErros)
        {
            return true;
        }
    }
}
