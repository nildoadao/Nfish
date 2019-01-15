using Nfish.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfish.Application.Common;

namespace Nfish.Application
{
    public class Storage
    {
        private IClient client;
        private IAuthenticator authenticator;

        public Storage(string host, IAuthenticator authenticator)
        {
            client = RestFactory.CreateClient();
            client.BaseUrl = new Uri(string.Format(@"https://{0}", host));
            this.authenticator = authenticator;
        }

        public async Task<IResponse> CreateVolumeAsync(string pushUri, string[] drives, Enums.VolumeType type)
        {
            IRequest request = RestFactory.CreateRequest();
            request.Resource = pushUri;
            request.Method = Method.POST;
            request.Format = DataFormat.Json;

            List<IDictionary<string, string>> volumeDrives = new List<IDictionary<string, string>>();
            
            foreach(string drive in drives)
            {
                IDictionary<string, string> item = new Dictionary<string, string>();
                item.Add("@odata.id", drive);
                volumeDrives.Add(item);
            }

            request.Parameters.Add("Drives", volumeDrives);
            request.Parameters.Add("VolumeType", type.ToString());
            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        public async Task<IResponse> CreateVolumeAsync(string pushUri, string[] drives, Enums.VolumeType type, long capacity, long ioSize, string name)
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

            request.Parameters.Add("Drives", volumeDrives);
            request.Parameters.Add("VolumeType", type.ToString());
            request.Parameters.Add("CapacityBytes", capacity);
            request.Parameters.Add("OptimumIOSizeBytes", ioSize);
            request.Parameters.Add("Name", name);

            client.Authenticate(authenticator, request);
            return await client.ExecuteAsync(request);
        }

        public async Task<IResponse> VolumeInitializeAsync(string target)
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
