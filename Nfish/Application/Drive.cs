using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application
{
    public class Drive
    {
        private IClient client;
        private IAuthenticator authenticator;

        public Drive(string host, string user, string password)
        {
            client = RestFactory.CreateClient();
            client.Host = host;
            authenticator = new BasicAuthenticator(user, password);
        }

        public async Task<IResponse> SecureEraseAsync(string target)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = target;
            request.Method = Method.POST;

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }
    }
}
