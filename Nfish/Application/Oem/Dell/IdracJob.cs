using Newtonsoft.Json.Linq;
using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application.Oem.Dell
{
    public class IdracJob
    {
        /// <summary>
        /// Create a new Idrac Job
        /// </summary>
        /// <param name="request">Request to create a Job</param>
        /// <returns>Uri for the Job created</returns>
        public async Task<string> CreateIdracJobAsync(IRequest request)
        {
            IClient client = RestFactory.CreateClient();
            IResponse response = await client.ExecuteAsync(request);
            return response.Headers["Location"].FirstOrDefault();
        }
    }
}
