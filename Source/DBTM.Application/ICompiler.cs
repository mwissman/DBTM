using DBTM.Domain.Entities;

namespace DBTM.Application
{
    public interface ICompiler
    {
        void CompileLatestVersion(Database database, string compiledSqlFolderPath, string databasePrefix);
        void CompileAllVersions(Database database,string compiledSqlFolderPath);
    }
}