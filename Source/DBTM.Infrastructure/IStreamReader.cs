using System.Reflection;

namespace DBTM.Infrastructure
{
    public interface IStreamReader
    {
        string ReadFile(string path);
        string ReadEmbeddedFile(Assembly resourceAssembly,string path);
    }
}