using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfish.Application.Common;

namespace Nfish.Application
{
    public class Storage
    {
        private IClient client;
        private IAuthenticator authenticator;

        /// <summary>
        /// Class to manage Storage tasks
        /// </summary>
        /// <param name="host">Hostname or Ip Address of the server</param>
        /// <param name="user">user for basic authentication</param>
        /// <param name="password">password for basic authentication</param>
        public Storage(string host, string user, string password)
        {
            client = RestFactory.CreateClient();
            client.Host = host;
            authenticator = new BasicAuthenticator(user, password);
        }

        public async Task<IResponse> SetEncryptionKeyAsync(string target, string key)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = target;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            request.BodyParameters.Add("EncryptionKey", key);

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }
    }
}
