using Nfish.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Nfish.Rest
{
    /// <summary>
    /// Class with basic information to access devices with Redfish API
    /// </summary>
    public class RestClient : IClient
    {
        /// <summary>
        /// Device's hostname or Ip address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Proxy to handle requests.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Enconding to perform requests, Default Encoding is ISO-8859-1
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Base Url to perform requests
        /// </summary>
        public Uri BaseUrl
        {
            get
            {
                return BaseUrl;
            }
            set
            {
                BaseUrl = value;
                Client.BaseAddress = value;
            }
        }

        private HttpClient Client { get; set; }

        public RestClient()
        {
            Encoding = Encoding.GetEncoding("ISO-8859-1");
            Client = HttpHelper.Create();
        }

        /// <summary>
        /// Authenticates an request
        /// </summary>
        /// <param name="authenticator">Authentication method</param>
        /// <param name="request">Request to be authenticated</param>
        public void Authenticate(IAuthenticator authenticator, IRequest request)
        {
            authenticator.Authenticate(request);
        }

        public async Task<IResponse> ExecuteAsync(IRequest request)
        {
            switch (request.Method)
            {
                case Method.GET:
                    return await ExecuteGetAsync(request);

                case Method.POST:
                    return await ExecutePostAsync(request);

                case Method.PUT:
                    return await ExecutePutAsync(request);

                case Method.DELETE:
                    return await ExecuteDeleteAsync(request);

                default:
                    return await ExecuteGetAsync(request);
            }
        }

        private void AddRequestHeaders(IRequest request, HttpRequestMessage requestMessage)
        {
            foreach (var header in request.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, string.Join(", ", header.Value));
            }
        }

        private async Task<IResponse> ExecutePostAsync(IRequest request)
        {
            if (request.Files.Count > 0)
                return await PostMultipartContent(request);
            else
                return await PostStringContent(request);
        }

        private async Task<IResponse> PostMultipartContent(IRequest request)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, request.Resource))
            using (MultipartFormDataContent multipartContent = new MultipartFormDataContent())
            {
                AddRequestHeaders(request, requestMessage);
                foreach (FileParameter file in request.Files)
                {
                    StreamContent fileContent = new StreamContent(File.Open(file.Path, FileMode.Open));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"",
                        FileName = string.Format("\"{0}\"", Path.GetFileName(file.Path))
                    };
                    multipartContent.Add(fileContent);
                }
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        private async Task<IResponse> PostStringContent(IRequest request)
        {
            string json = "";

            if (request.Parameters.Count > 0)
                json = JsonConvert.SerializeObject(request.Parameters, Formatting.Indented);
            else
                json = request.JsonBody;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, request.Resource))
            using (StringContent stringContent = new StringContent(json, Encoding, "application/json"))
            {
                AddRequestHeaders(request, requestMessage);
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        private async Task<IResponse> ExecuteGetAsync(IRequest request)
        {
            using(HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, request.Resource))
            {
                AddRequestHeaders(request, requestMessage);
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        private async Task<IResponse> ExecuteDeleteAsync(IRequest request)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, request.Resource))
            {
                AddRequestHeaders(request, requestMessage);
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        private async Task<IResponse> ExecutePutAsync(IRequest request)
        {
            if (request.Files.Count > 0)
                return await PutMultipartContent(request);
            else
                return await PutStringContent(request);
        }

        private async Task<IResponse> PutMultipartContent(IRequest request)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, request.Resource))
            using (MultipartFormDataContent multipartContent = new MultipartFormDataContent())
            {
                AddRequestHeaders(request, requestMessage);
                foreach (FileParameter file in request.Files)
                {
                    StreamContent fileContent = new StreamContent(File.Open(file.Path, FileMode.Open));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"",
                        FileName = string.Format("\"{0}\"", Path.GetFileName(file.Path))
                    };
                    multipartContent.Add(fileContent);
                }
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        private async Task<IResponse> PutStringContent(IRequest request)
        {
            string json = "";

            if (request.Parameters.Count > 0)
                json = JsonConvert.SerializeObject(request.Parameters, Formatting.Indented);
            else
                json = request.JsonBody;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, request.Resource))
            using (StringContent stringContent = new StringContent(json, Encoding, "application/json"))
            {
                AddRequestHeaders(request, requestMessage);
                using (HttpResponseMessage responseMessage = await Client.SendAsync(requestMessage))
                {
                    IResponse response = RestFactory.CreateResponse();
                    response.StatusCode = (int)responseMessage.StatusCode;
                    response.RequestMessage = request;
                    response.Headers = requestMessage.Headers.ToDictionary(x => x.Key, x => x.Value);
                    response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }
    }
}
