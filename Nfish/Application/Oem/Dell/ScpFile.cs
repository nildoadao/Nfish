﻿using Nfish.Rest;
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

        private const string exportUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ExportSystemConfiguration";
        private const string importUri = @"/redfish/v1/Managers/iDRAC.Embedded.1/Actions/Oem/EID_674_Manager.ImportSystemConfiguration";

        private IClient client;
        private IAuthenticator authenticator;
        
        /// <summary>
        /// Class to manage Scp Files
        /// </summary>
        /// <param name="host">Hostname or Ip Address of the server</param>
        /// <param name="authenticator">Authentication method</param>
        public ScpFile(string host, IAuthenticator authenticator)
        {
            client = RestFactory.CreateClient();
            client.BaseUrl = new Uri(string.Format(@"https://{0}", host));
            this.authenticator = authenticator;
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
    
            request.Parameters.Add("ExportFormat", format);
            request.Parameters.Add("SharedParameters", new { Target = target });

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
            request.Parameters.Add("ImportBuffer", fileData);
            request.Parameters.Add("SharedParameters", new { Target = target });
            request.Parameters.Add("ShutdownType", shutdownType);
            request.Parameters.Add("HostPowerState", powerState);

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