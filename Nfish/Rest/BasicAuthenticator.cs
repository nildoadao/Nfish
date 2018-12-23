using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public class BasicAuthenticator : IAuthenticator
    {
        private string authentication;

        public BasicAuthenticator(string user, string password)
        {
            authentication = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + password));            
        }
        public void Authenticate(IRequest request)
        {
            if (!request.Headers.Keys.Contains("Authorization"))
                request.AddHeader("Authorization", string.Format("Basic {0}", authentication));
        }
    }
}
