using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfish.Rest;
using Nfish.Application.Common;
using System.IO;
using Newtonsoft.Json.Linq;
using Nfish.Util;

namespace Nfish.Application
{
    public class UpdateService
    {
        private IClient client;
        private IAuthenticator authenticator;

        /// <summary>
        /// Class to manage UpdateServices taks.
        /// </summary>
        /// <param name="host">Hostname or Ip Address of the server</param>
        /// <param name="user">User for basic authentication</param>
        /// <param name="password">Password for basic authentication</param>
        public UpdateService(string host, string user, string password)
        {
            client = RestFactory.CreateClient();
            client.Host = host;
            authenticator = new BasicAuthenticator(user, password);
        }

        public async Task<string> GetUpdateServiceUriAsync()
        {
            RedfishCrawler crawler = new RedfishCrawler(client.Host, authenticator);
            await crawler.Crawl();
            return crawler.Resources["UpdateService"];
        }

        /// <summary>
        /// Performs a DTMF software update
        /// </summary>
        /// <param name="image">The URI of the software image to be installed</param>
        /// <param name="targets">The array of URIs indicating where the update image is to be applied</param>
        /// <param name="protocol">The network protocol used by the Update Service</param>
        /// <returns>Rest response of the update request</returns>
        public async Task<IResponse> SimpleUpdateAsync(string image, IEnumerable<string> targets, Enums.TransferProtocol protocol)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = await GetUpdateServiceUriAsync();
            request.Method = Method.POST;
            request.BodyParameters.Add("ImageURI", image);
            request.BodyParameters.Add("Targets", targets);
            request.BodyParameters.Add("TransferProtocol", protocol);
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
            request.BodyParameters.Add("ImageURI", image);
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
