using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfish.Rest;
using Nfish.Application.Common;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Nfish.Application
{
    public class UpdateService
    {
        private IClient client;
        private IAuthenticator authenticator;

        /// <summary>
        /// Creates a new instance of UpdateService
        /// </summary>
        /// <param name="host">Device hostname or ip address</param>
        /// <param name="authenticator">Authentication method</param>
        public UpdateService(string host, IAuthenticator authenticator)
        {
            client = RestFactory.CreateClient();
            client.BaseUrl = new Uri(string.Format(@"https://{0}", host));
            this.authenticator = authenticator;
        }

        public async Task<string> GetUpdateServiceUriAsync()
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = @"/redfish/v1";
            request.Method = Method.GET;
            client.Authenticate(authenticator, request);
            IResponse response = await client.ExecuteAsync(request);
            JObject json = JObject.Parse(response.JsonContent);
            return json["UpdateService"]["@odata.id"].ToString();
        }

        /// <summary>
        /// Performs a DTMF software update
        /// </summary>
        /// <param name="image">The URI of the software image to be installed</param>
        /// <param name="targets">The array of URIs indicating where the update image is to be applied</param>
        /// <param name="protocol">The network protocol used by the Update Service</param>
        /// <returns>Rest response of the update request</returns>
        public async Task<IResponse> SimpleUpdateAsync(string image, string[] targets, Enums.TransferProtocol protocol)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = await GetUpdateServiceUriAsync();
            request.Method = Method.POST;
            request.Parameters.Add("ImageURI", image);
            request.Parameters.Add("Targets", targets);
            request.Parameters.Add("TransferProtocol", protocol);
            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Performs a DTMF software update
        /// </summary>
        /// <param name="image">The URI of the software image to be installed</param>
        /// <returns>Rest response of the update request</returns>
        public async Task<IResponse> SimpleUpdateAsync(string image)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = await GetUpdateServiceUriAsync();
            request.Method = Method.POST;
            request.Parameters.Add("ImageURI", image);
            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Upload an file to an given uri
        /// </summary>
        /// <param name="path">Local path of the file</param>
        /// <returns>Rest response of the upload</returns>
        public async Task<IResponse> UploadFileAsync(string path)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = await GetUpdateServiceUriAsync();
            request.Method = Method.POST;
            FileParameter file = new FileParameter(path, Path.GetFileName(path), "multipart/form-data");
            request.AddFile(file);
            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Upload an file to an given uri
        /// </summary>
        /// <param name="path">Local path of the file</param>
        /// <param name="headers">Custom headers for the upload request</param>
        /// <returns>Rest response of the update</returns>
        public async Task<IResponse> UploadFileAsync(string path, IDictionary<string, IList<string>> headers)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = await GetUpdateServiceUriAsync();
            request.Method = Method.POST;
            FileParameter file = new FileParameter(path, Path.GetFileName(path), "multipart/form-data");
            request.AddFile(file);

            foreach (var header in headers)
                request.Headers.Add(header);

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }
    }
}
