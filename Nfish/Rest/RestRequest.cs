using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public class RestRequest : IRequest
    {
        public Method Method { get; set; }

        public List<FileParameter> Files { get; }

        public IDictionary<string, object> Parameters { get; }

        public string JsonBody { get; set; }

        public IDictionary<string, IList<string>> Headers { get; }

        public string Resource { get; set; }

        public DataFormat Format { get; set; }

        public RestRequest()
        {
            Files = new List<FileParameter>();
            Headers = new Dictionary<string, IList<string>>();
            Parameters = new Dictionary<string, object>();
            JsonBody = "";
            Method = Method.GET;
            Format = DataFormat.Json;
        }

        public RestRequest(string resource) : this()
        {
            Resource = resource;            
        }

        public RestRequest(string resource, Method method) : this()
        {
            Resource = resource;
            Method = method;
        }

        public IRequest AddFile(FileParameter file)
        {
            Files.Add(file);
            return this;
        }

        public IRequest AddHeader(string name, string value)
        {
            if(!Headers.TryGetValue(name, out IList<string> values))
            {
                values = new List<string>() { value };
                Headers.Add(name, values);
            }
            else
            {
                values = Headers[name];
                values.Add(value);
            }
            return this;
        }

        public IRequest AddJsonBody(string json)
        {
            JsonBody = json;
            return this;
        }

        public IRequest AddJsonBody(object body)
        {
            JsonBody = JsonConvert.SerializeObject(body);
            return this;
        }

        public IRequest AddParameter(string name, object value)
        {
            Parameters.Add(name, value);
            return this;
        }
    }
}
