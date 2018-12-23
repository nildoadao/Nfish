using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public interface IAuthenticator
    {
        void Authenticate(IRequest request);
    }
}
