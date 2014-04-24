using System.IO;
using System.Linq;

namespace DBTM.Application
{
    public class SqlFileReader : ISqlFileReader
    {
        public ISqlFileList GetFromDirectoryPath(string directoryPath)
        {
            return new SqlFileList(Directory.GetFiles(directoryPath, "*.sql").OrderBy(x=> x).ToList());
        }
    }
}