using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application.Oem.Dell
{
    public class ScpFile
    {
        /// <summary>
        /// Uri to import/export SCP Files
        /// </summary>
        private const string exportUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ExportSystemConfiguration";
        private const string importUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ImportSystemConfiguration";

        private IClient client;
        private IAuthenticator authenticator;

        /// <summary>
        /// Class to manage Scp Files
        /// </summary>
        /// <param name="host">Hostname or Ip Address of the server</param>
        /// <param name="user">user for basic authentication</param>
        /// <param name="password">password for basic authentication</param>
        public ScpFile(string host, string user, string password)
        {
            client = RestFactory.CreateClient();
            client.Host = host;
            authenticator = new BasicAuthenticator(user, password);
        }

        /// <summary>
        /// Exports an Scp File to a local file.
        /// </summary>
        /// <param name="format">File format</param>
        /// <param name="target">Items to add in the file</param>
        /// <returns>Rest response of export</returns>
        public async Task<IResponse> ExportLocalScpFileAsync(string format, string target)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = exportUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;
    
            request.BodyParameters.Add("ExportFormat", format);
            request.BodyParameters.Add("SharedParameters", new { Target = target });

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Imports an Scp configuration from a local file.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="target">Items who will be imported in the file</param>
        /// <param name="shutdownType">Shutdown method to apply the configurations</param>
        /// <param name="powerState">Server current power state.</param>
        /// <returns>Rest response of import</returns>
        public async Task<IResponse> ImportLocalScpFileAsync(string path, string target, string shutdownType, string powerState)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = importUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            string fileData = File.ReadAllText(path);
            request.BodyParameters.Add("ImportBuffer", fileData);
            request.BodyParameters.Add("SharedParameters", new { Target = target });
            request.BodyParameters.Add("ShutdownType", shutdownType);
            request.BodyParameters.Add("HostPowerState", powerState);

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Save to an local file an Scp File previously exported
        /// </summary>
        /// <param name="path">Local to save the file</param>
        /// <param name="location">Path to the export Job</param>
        /// <returns></returns>
        public async Task SaveLocalScpFileAsync(string path, string location)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = location;
            request.Method = Method.GET;
            request.Format = DataFormat.Json;

            client.Authenticate(authenticator, request);
            IResponse response = await client.ExecuteAsync(request);
            File.WriteAllText(path, response.JsonContent);
        }
    }
}
