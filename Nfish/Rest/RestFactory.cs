using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public class RestFactory
    {
        public static IClient CreateClient()
        {
            return new RestClientAsync();
        }

        public static IRequest CreateRequest()
        {
            return new RestRequest();
        }

        public static IResponse CreateResponse()
        {
            return new RestResponse();
        }
    }
}
