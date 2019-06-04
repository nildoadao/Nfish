using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public interface IRequest
    {
        Method Method { get; set; }
        List<FileParameter> Files { get; }
        IDictionary<string, object> BodyParameters { get; }
        IDictionary<string, object> QueryParameters { get; }
        string JsonBody { get; }
        IDictionary<string, IList<string>> Headers { get; }
        string Resource { get; set; }
        DataFormat Format { get; set; }
        IRequest AddJsonBody(object body);
        IRequest AddJsonBody(string json);
        IRequest AddFile(FileParameter file);
        IRequest AddHeader(string name, string value);
    }
}
