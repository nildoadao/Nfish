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
using System.Web;

namespace Nfish.Rest
{
    /// <summary>
    /// Class with basic information to access devices with Redfish API
    /// </summary>
    public class RestClientAsync : IClient
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

        public RestClientAsync()
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
            HttpMethod method = null;

            switch (request.Method)
            {
                case Method.GET:
                    method = HttpMethod.Get;
                    break;

                case Method.POST:
                    method = HttpMethod.Post;
                    break;

                case Method.PUT:
                    method = HttpMethod.Put;
                    break;

                case Method.DELETE:
                    method = HttpMethod.Delete;
                    break; ;

                case Method.PATCH:
                    method = new HttpMethod("PATCH");
                    break;

                default:
                    method = HttpMethod.Get;
                    break;
            }

            string targetUri;

            if (request.QueryParameters.Any())
                targetUri = string.Concat(baseUrl, request.Resource, BuildQueryString(request).Result);
            else
                targetUri = string.Concat(baseUrl, request.Resource);

            if (request.BodyParameters.Any() || request.Files.Any())
                return await ExecuteMultipartRequestAsync(request, method, targetUri);
            else
                return await ExecuteSimpleRequestAsync(request, method, targetUri);
        }

        private async Task<string> BuildQueryString(IRequest request)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();

            foreach (var parameter in request.QueryParameters)
                queryParameters.Add(parameter.Key, parameter.Value.ToString());

            var content = new FormUrlEncodedContent(queryParameters);
            return await content.ReadAsStringAsync();
        }

        private void AddRequestHeaders(IRequest request, HttpRequestMessage requestMessage)
        {
            foreach (var header in request.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, string.Join(", ", header.Value));
            }
        }
             
        private async Task<IResponse> ExecuteMultipartRequestAsync(IRequest request, HttpMethod method, string uri)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri))
            using (MultipartFormDataContent multipartContent = BuildMultipartBody(request))
            {
                AddRequestHeaders(request, requestMessage);
                requestMessage.Content = multipartContent;
                return await GetResponseAsync(requestMessage, request);
            }
        }

        private MultipartFormDataContent BuildMultipartBody(IRequest request)
        {
            var boundary = Guid.NewGuid().ToString();
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);

            //Some implementations don't accept boundary in quotes.
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

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

            string format = request.Format == DataFormat.Json ? @"application/json" : @"application/xml";
            string body = string.Empty;

            if (request.Format == DataFormat.Json)
            {
                if (request.BodyParameters.Any())
                    body += JsonConvert.SerializeObject(request.BodyParameters, Formatting.Indented);

                if (!string.IsNullOrEmpty(request.JsonBody))
                    body += request.JsonBody;
            }
            else
            {
                if (request.BodyParameters.Any())
                {
                    using (StringWriter xml = new StringWriter())
                    {
                        XmlSerializer parser = new XmlSerializer(request.BodyParameters.GetType());
                        parser.Serialize(xml, request.BodyParameters);
                        body += xml.ToString();
                    }
                }
            }

            StringContent stringContent = new StringContent(body, Encoding, format);
            content.Add(stringContent);
            return content;
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

        private async Task<IResponse> ExecuteSimpleRequestAsync(IRequest request, HttpMethod method, string uri)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri))
            {
                AddRequestHeaders(request, requestMessage);
                return await GetResponseAsync(requestMessage, request);
            }
        }
    }
}
