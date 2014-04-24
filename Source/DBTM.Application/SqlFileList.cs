using System.Collections.Generic;
using DBTM.Infrastructure;

namespace DBTM.Application
{
    public class SqlFileList : ISqlFileList
    {
        public SqlFileList(IList<string> fileNames)
        {
            Files = new List<ISqlFile>();

            foreach (var fileName in fileNames)
            {
                Files.Add(new SqlFile(ReadContents(fileName), fileName));
            }
        }

        private string ReadContents(string fileName)
        {
            return new StreamReader().ReadFile(fileName);
        }

        public IList<ISqlFile> Files 
        { 
            get;
            private set;
        }
    }
}