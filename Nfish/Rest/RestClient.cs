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
        private string baseUrl;

        /// <summary>
        /// Device's hostname or Ip address.
        /// </summary>
        public string Host
        {
            get { return host; }

            set
            {
                host = value;
                baseUrl = string.Format(@"https://{0}", host);
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

                case Method.PATCH:
                    return await ExecutePatchAsync(request);

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
            using(HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl + request.Resource))
            {
                AddRequestHeaders(request, requestMessage);
                return await GetResponseAsync(requestMessage, request);
            }
        }

        private async Task<IResponse> ExecuteDeleteAsync(IRequest request)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, baseUrl + request.Resource))
            {
                AddRequestHeaders(request, requestMessage);
                return await GetResponseAsync(requestMessage, request);
            }
        }

        private async Task<IResponse> ExecutePatchAsync(IRequest request)
        {
            return await StringRequestAsync(request, Method.PATCH);
        }
             
        private async Task<IResponse> MultipartRequestAsync(IRequest request, Method method)
        {
            HttpMethod type = method == Method.POST ? HttpMethod.Post : HttpMethod.Put;

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(type, baseUrl + request.Resource))
            using (MultipartFormDataContent multipartContent = new MultipartFormDataContent())
            {
                AddRequestHeaders(request, requestMessage);
                BuildMultipartBody(multipartContent, request, method);

                if (request.Parameters.Count > 0)
                {
                    string body = BuildStringBody(request, method);
                    string format = request.Format == DataFormat.Json ? @"application/json" : @"application/xml";
                    multipartContent.Add(new StringContent(body, Encoding, format));
                }

                requestMessage.Content = multipartContent;
                return await GetResponseAsync(requestMessage, request);
            }
        }

        private HttpContent BuildMultipartBody(MultipartFormDataContent content, IRequest request, Method method)
        {
            foreach (FileParameter file in request.Files)
            {
                StreamContent fileContent = new StreamContent(File.Open(file.Path, FileMode.Open));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = string.Format("\"{0}\"", Path.GetFileName(file.Path))
                };
                content.Add(fileContent);
            }
            return content;
        }

        private string BuildStringBody(IRequest request, Method method)
        {            
            string body = "";

            if (request.Format == DataFormat.Json)
            {
                if (request.Parameters.Count > 0)
                    body += JsonConvert.SerializeObject(request.Parameters, Formatting.Indented);

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
            return body;
        }

        private async Task<IResponse> GetResponseAsync(HttpRequestMessage message, IRequest request)
        {
            using (HttpResponseMessage responseMessage = await Client.SendAsync(message))
            {
                IResponse response = RestFactory.CreateResponse();
                response.StatusCode = (int)responseMessage.StatusCode;
                response.RequestMessage = request;
                response.Headers = message.Headers.ToDictionary(x => x.Key, x => x.Value);
                response.JsonContent = await responseMessage.Content.ReadAsStringAsync();
                return response;
            }
        }

        private async Task<IResponse> StringRequestAsync(IRequest request, Method method)
        {            
            string format = request.Format == DataFormat.Json ? @"application/json" : @"application/xml";
            HttpMethod type = null;

            switch (method)
            {
                case Method.POST:
                    type = HttpMethod.Post;
                    break;
                case Method.PUT:
                    type = HttpMethod.Put;
                    break;
                case Method.PATCH:
                    type = new HttpMethod("PATCH");
                    break;
            }

            string body = BuildStringBody(request, method);

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(type, baseUrl + request.Resource))
            using (StringContent stringContent = new StringContent(body, Encoding, format))
            {
                AddRequestHeaders(request, requestMessage);
                requestMessage.Content = stringContent;
                return await GetResponseAsync(requestMessage, request);
            }
        }
    }
}
