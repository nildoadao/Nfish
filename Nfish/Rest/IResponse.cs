using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public interface IResponse
    {
        int StatusCode { get; set; }
        IRequest RequestMessage { get; set; }
        string JsonContent { get; set; }
        IDictionary<string, IEnumerable<string>> Headers { get; set; }
        bool IsSuccessfull();
    }
}
