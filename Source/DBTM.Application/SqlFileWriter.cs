using System.IO;
using DBTM.Domain.Entities;
using DBTM.Infrastructure;

namespace DBTM.Application
{
    public class SqlFileWriter : ISqlFileWriter
    {
        private readonly IStreamWriter _streamWriter;

        public SqlFileWriter(IStreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Write(ICompiledSql compiledSql, string upgradeFilePath, string rollbackFilePath)
        {
            CreatePathIfNonexsistent(upgradeFilePath);
            CreatePathIfNonexsistent(rollbackFilePath);

            _streamWriter.Write(upgradeFilePath, compiledSql.Upgrade);
            _streamWriter.Write(rollbackFilePath, compiledSql.Rollback);
        }

        private void CreatePathIfNonexsistent(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
        }
    }
}