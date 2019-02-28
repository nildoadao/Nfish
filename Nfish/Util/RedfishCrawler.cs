using Newtonsoft.Json.Linq;
using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Util
{
    /// <summary>
    /// Class to Map an Redfish host in order to get an list to his resources Uris.
    /// </summary>
    public class RedfishCrawler
    {
        private string host;
        private IAuthenticator authenticator;
        public Dictionary<string, string> Resources { get; private set; }
        private List<string> urisFound;
        private List<string> urisFollowed;
        private List<string> urisToAdd;
        private IClient client;

        /// <summary>
        /// Creates a new Crawler.
        /// </summary>
        /// <param name="host">Hostname or Ip address</param>
        /// <param name="user">User for basic authentication</param>
        /// <param name="password">Password for basic authentication</param>
        public RedfishCrawler(string host, string user, string password)
        {
            this.host = host;
            authenticator = new BasicAuthenticator(user, password);
            Resources = new Dictionary<string, string>();
            urisFound = new List<string>();
            urisFollowed = new List<string>();
            urisToAdd = new List<string>();
            client = RestFactory.CreateClient();
            client.Host = host;
        }

        /// <summary>
        /// Creates a new Crawler.
        /// </summary>
        /// <param name="host">Hostname or Ip address</param>
        /// <param name="authenticator">Authentication object</param>
        public RedfishCrawler(string host, IAuthenticator authenticator)
        {
            this.host = host;
            this.authenticator = authenticator;
            Resources = new Dictionary<string, string>();
            urisFound = new List<string>();
            urisFollowed = new List<string>();
            client = RestFactory.CreateClient();
            client.Host = host;
        }

        /// <summary>
        /// Maps an given uri looking for @odate.id links.
        /// </summary>
        /// <param name="uri">Uri to map.</param>
        /// <returns></returns>
        private async Task MapUri(string uri)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = uri;
            request.Method = Method.GET;
            client.Authenticate(authenticator, request);
            IResponse response = await client.ExecuteAsync(request);
            urisFollowed.Add(uri);

            JObject json = JObject.Parse(response.JsonContent);

            string jsonId = json["Id"] == null ? string.Empty : json["Id"].ToString();
            string odataId = json["@odata.id"] == null ? string.Empty : json["@odata.id"].ToString();

            if(!Resources.Keys.Contains(jsonId) && !String.IsNullOrEmpty(jsonId)
                && !String.IsNullOrEmpty(odataId))
                Resources.Add(jsonId, odataId);

            foreach (JProperty item in json.Children())
                GetLinks(item);
        }

        /// <summary>
        /// Adds the uris found in urisFound List.
        /// </summary>
        /// <param name="item">Json item to inspect.</param>
        private void GetLinks(JToken item)
        {
            if (item.HasValues)
            {
                foreach (var subItem in item.Children())
                    GetLinks(subItem);
            }
            if (item is JProperty)
            {
                JProperty property = (JProperty)item;

                if (property.Name.Equals("@odata.id") && !urisFound.Contains(property.Value.ToString()))
                    urisToAdd.Add(property.Value.ToString());
            }
        }

        /// <summary>
        /// Performs an mapping through all uris looking for resources, the resources found are stored in Resources property. 
        /// </summary>
        /// <returns></returns>
        public async Task Crawl()
        {
            // Begin the mapping from Redfish Root
            await MapUri(@"/redfish/v1");

            foreach (string uri in urisToAdd)
                if (!urisFound.Contains(uri))
                    urisFound.Add(uri);

            urisToAdd.Clear();

            while (urisFound.Count > urisFollowed.Count)
            {
                foreach(string uri in urisFound)
                {
                    if (!urisFollowed.Contains(uri))
                        await MapUri(uri);
                }

                foreach (string uri in urisToAdd)
                {
                    if(!urisFound.Contains(uri))
                        urisFound.Add(uri);
                }
                urisToAdd.Clear();
            }
        }
    }
}
