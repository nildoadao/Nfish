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
using System.Xml.Serialization;

namespace Nfish.Rest
{
    /// <summary>
    /// Class with basic information to access devices with Redfish API
    /// </summary>
    public class RestClient : IClient
    {
        private string host;
        private Uri baseUrl;

        /// <summary>
        /// Device's hostname or Ip address.
        /// </summary>
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
                baseUrl = new Uri(host);
            }
        }

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
                return baseUrl;
            }
            set
            {
                baseUrl = value;
                Client.BaseAddress = baseUrl;
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
                    return await ExecutePostOrPutAsync(request);

                case Method.PUT:
                    return await ExecutePostOrPutAsync(request);

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

        private async Task<IResponse> ExecutePostOrPutAsync(IRequest request)
        {
            if (request.Files.Count > 0)
                return await MultipartRequestAsync(request, request.Method);
            else
                return await StringRequestAsync(request, request.Method);
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

        private async Task<IResponse> MultipartRequestAsync(IRequest request, Method method)
        {
            HttpMethod type = method == Method.POST ? HttpMethod.Post : HttpMethod.Put;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(type, request.Resource))
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

        private async Task<IResponse> StringRequestAsync(IRequest request, Method method)
        {
            HttpMethod type = method == Method.POST ? HttpMethod.Post : HttpMethod.Put;
            string format = request.Format == DataFormat.Json ? @"application/json" : @"application/xml";

            string body = "";

            if(request.Format == DataFormat.Json)
            {
                if (request.Parameters.Count > 0)
                {
                    body += JsonConvert.SerializeObject(request.Parameters, Formatting.Indented);
                }
                if (!String.IsNullOrEmpty(request.JsonBody))
                    body += request.JsonBody;
            }
            else
            {
                if (request.Parameters.Count > 0)
                {
                    using (StringWriter xml = new StringWriter())
                    {
                        XmlSerializer parser = new XmlSerializer(request.Parameters.GetType());
                        parser.Serialize(xml, request.Parameters);
                        body += xml.ToString();
                    }
                }
            }

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(type, request.Resource))
            using (StringContent stringContent = new StringContent(body, Encoding, format))
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
