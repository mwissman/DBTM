using System;
using DBTM.Domain;
using DBTM.Domain.Entities;
using DBTM.Infrastructure;

namespace DBTM.Application
{
    public class SqlScriptRepository : ISqlScriptRepository
    {
        private readonly IStreamReader _reader;

        public SqlScriptRepository(IStreamReader reader)
        {
            _reader = reader;
        }

        public string LoadDisconnectUser(string databaseName)
        {
            string disconnectTemplate =_reader.ReadEmbeddedFile(System.Reflection.Assembly.GetExecutingAssembly(),".SqlScripts.Disconnect.sql");

            return disconnectTemplate.Replace("[DATABASE_NAME]", databaseName);
        }

        public string LoadCreateSchema(string databaseName, string databaseFilesDirectory)
        {
            string createTemplate = _reader.ReadEmbeddedFile(System.Reflection.Assembly.GetExecutingAssembly(),".SqlScripts.CreateSchema.sql");

            return createTemplate.Replace("[DATABASE_NAME]", databaseName).Replace("[DATABASE_FILE_DIRECTORY]",databaseFilesDirectory);
        }

        public string LoadDropSchema(string databaseName)
        {
            string createTemplate = _reader.ReadEmbeddedFile(System.Reflection.Assembly.GetExecutingAssembly(),".SqlScripts.DropSchema.sql");

            return createTemplate.Replace("[DATABASE_NAME]", databaseName);
        }

        public ICompiledSql LoadHistroySql()
        {
            string rollbackSql = _reader.ReadEmbeddedFile(System.Reflection.Assembly.GetExecutingAssembly(), ".SqlScripts.DBTMHistoryRollback.sql");
            string upgradeSql = _reader.ReadEmbeddedFile(System.Reflection.Assembly.GetExecutingAssembly(), ".SqlScripts.DBTMHistoryUpgrade.sql");

            return new GoSeperatedCompiledSql(upgradeSql, rollbackSql);
        }
    }
}