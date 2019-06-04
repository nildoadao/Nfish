using Nfish.Application.Common;
using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application
{
    public class Volume
    {
        private IClient client;
        private IAuthenticator authenticator;

        public Volume(string host, string user, string password)
        {
            client = RestFactory.CreateClient();
            client.Host = host;
            authenticator = new BasicAuthenticator(user, password);
        }

        public async Task<IResponse> CreateVolumeAsync(string pushUri, IEnumerable<string> drives, Enums.VolumeType type)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = pushUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            List<IDictionary<string, string>> volumeDrives = new List<IDictionary<string, string>>();

            foreach (string drive in drives)
            {
                IDictionary<string, string> item = new Dictionary<string, string>();
                item.Add("@odata.id", drive);
                volumeDrives.Add(item);
            }

            request.BodyParameters.Add("Drives", volumeDrives);
            request.BodyParameters.Add("VolumeType", type.ToString());
            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        public async Task<IResponse> CreateVolumeAsync(string pushUri, IEnumerable<string> drives, Enums.VolumeType type, long capacity, long ioSize, string name)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = pushUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            List<IDictionary<string, string>> volumeDrives = new List<IDictionary<string, string>>();

            foreach (string drive in drives)
            {
                IDictionary<string, string> item = new Dictionary<string, string>();
                item.Add("@odata.id", drive);
                volumeDrives.Add(item);
            }

            request.BodyParameters.Add("Drives", volumeDrives);
            request.BodyParameters.Add("VolumeType", type.ToString());
            request.BodyParameters.Add("CapacityBytes", capacity);
            request.BodyParameters.Add("OptimumIOSizeBytes", ioSize);
            request.BodyParameters.Add("Name", name);

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        public async Task<IResponse> InitializeAsync(string target)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = target;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }
    }
}
