using System;
using System.IO;
using System.Reflection;

namespace DBTM.Infrastructure
{
    public class StreamReader : IStreamReader
    {
        public string ReadFile(string path)
        {
            using (var streamReader = new System.IO.StreamReader(path))
            {
                return streamReader.ReadToEnd();
            }
        }

        public string ReadEmbeddedFile(Assembly resourceAssembly, string path)
        {
            string resourceName = resourceAssembly.GetName().Name + path;

            Stream resourceStream = resourceAssembly.GetManifestResourceStream(resourceName);
            if (resourceStream != null)
            {
                using (var streamReader = new System.IO.StreamReader(resourceStream))
                {
                    return streamReader.ReadToEnd();
                }

            }

            return string.Empty;
        }
    }
}