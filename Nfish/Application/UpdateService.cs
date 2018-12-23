using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfish.Rest;
using Nfish.Application.Common;
using System.IO;

namespace Nfish.Application
{
    public class UpdateService
    {
        private IClient client;
        private IAuthenticator authenticator;

        public UpdateService(string host, IAuthenticator auth)
        {
            client = new RestClient(string.Format(@"https://{0}", host));
            authenticator = auth;
        }
        
        public Task<IResponse> SimpleUpdate(string image, string[] targets, Enums.TransferProtocol protocol)
        {
            IRequest request = new RestRequest(@"/redfish/v1/UpdateService/Actions/SimpleUpdate", Method.POST);
            request.Parameters.Add("ImageURI", image);
            request.Parameters.Add("Targets", targets);
            request.Parameters.Add("TransferProtocol", protocol);
            client.Authenticate(authenticator, request);
            return client.ExecuteAsync(request);
        }

        public async Task<Uri> UploadFile(string pushUri, string path)
        {
            IRequest request = new RestRequest(pushUri, Method.POST);
            FileParameter file = new FileParameter(path, Path.GetFileName(path), "multipart/form-data");
            request.AddFile(file);
            client.Authenticate(authenticator, request);
            IResponse response = await client.ExecuteAsync(request);
            response.Headers.TryGetValue("Location", out IEnumerable<string> values);
            return new Uri(values.FirstOrDefault());
        }
    }
}
