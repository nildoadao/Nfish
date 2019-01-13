using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application.Oem.Dell
{
    public class ScpFile
    {

        private const string exportUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ExportSystemConfiguration";
        private const string importUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ImportSystemConfiguration";

        private IClient client;
        private IAuthenticator authenticator;
        

        public ScpFile(string host, IAuthenticator authenticator)
        {
            client = RestFactory.CreateClient();
            client.BaseUrl = new Uri(string.Format(@"https://{0}", host));
            this.authenticator = authenticator;
        }

        public async Task<string> ExportLocalScpFileAsync(string format, string target)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = exportUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;
    
            request.Parameters.Add("ExportFormat", format);
            var sharedParameters = new
            {
                Target = target
            };
            request.Parameters.Add("SharedParameters", sharedParameters);

            client.Authenticate(authenticator, request);
            IResponse response = await client.ExecuteAsync(request);
            response.Headers.TryGetValue("Location", out IEnumerable<string> values);
            return values.FirstOrDefault();
        }
    }
}
