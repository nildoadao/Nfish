using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public interface IClient
    {
        string Host { get; set; }
        IWebProxy Proxy { get; set; }
        Encoding Encoding { get; set; }
        void Authenticate(IAuthenticator authenticator, IRequest request);
        Task<IResponse> ExecuteAsync(IRequest request);
    }
}
