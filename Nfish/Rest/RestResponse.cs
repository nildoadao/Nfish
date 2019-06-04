using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public class RestResponse : IResponse
    {
        public int StatusCode { get; set; }

        public IRequest RequestMessage { get; set; }

        public string JsonContent { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public bool IsSuccessfull()
        {
            if (StatusCode >= 200 & StatusCode <= 299)
                return true;
            else
                return false;
        }
        public RestResponse()
        {
            StatusCode = 0;
            JsonContent = "";
            Headers = new Dictionary<string, IEnumerable<string>>();
        }
    }
}
