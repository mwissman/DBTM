using DBTM.Domain.Entities;

namespace DBTM.Application
{
    public interface ISqlScriptRepository
    {
        string LoadDisconnectUser(string databaseName);
        string LoadCreateSchema(string databaseName, string databaseFilesDirectory);
        string LoadDropSchema(string databaseName);
        ICompiledSql LoadHistroySql();
    }
}