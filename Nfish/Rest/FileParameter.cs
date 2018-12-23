using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Rest
{
    public class FileParameter
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public FileParameter(string path, string name, string contentType = null)
        {
            Path = path;
            Name = name;
            ContentType = String.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType;
        }
    }
}
