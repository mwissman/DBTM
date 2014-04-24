using DBTM.Application.SQL;
using DBTM.Domain.Entities;
using System.Linq;

namespace DBTM.Application
{
    public class DatabaseBuildService : IDatabaseBuildService
    {
        private readonly ISqlRunner _sqlRunner;
        private readonly ISqlServerDatabase _sqlServerDatabase;
        private readonly ISqlScriptRepository _scriptRepository;
        private const string BUILD_SUCCESS_MESSAGE = "Build Successful!";
        private const string BUILD_FAILED_TEMPLATE = "Build Failed!\n\nSql Server Message:\n{0}\n\nCommand Text:\n{1}\n\nConnection String:\n{2}\n";

        public DatabaseBuildService(ISqlRunner sqlRunner, ISqlServerDatabase sqlServerDatabase, ISqlScriptRepository scriptRepository)
        {
            _sqlRunner = sqlRunner;
            _sqlServerDatabase = sqlServerDatabase;
            _scriptRepository = scriptRepository;
        }

        public DatabaseBuildResult FullBuild(Database database, ISqlServerDatabaseSettings sqlServerDatabaseSettings)
        {
            
            if (database.Versions.Count > 0)
            {
                _sqlServerDatabase.Initialize(sqlServerDatabaseSettings);
            }
            try
            {
                string connectionString = sqlServerDatabaseSettings.ConnectionString;
                string prefix = sqlServerDatabaseSettings.CrossDatabaseNamePrefix;

                var compiledHistorySql=_scriptRepository.LoadHistroySql();

                _sqlRunner.RunUpgrade(compiledHistorySql,connectionString);

                var databaseVersions = database.Versions.OrderBy(db => db.VersionNumber).ToList();
                databaseVersions.ForEach(dbv =>
                                {
                                    bool includeHistory = dbv.VersionNumber >= database.BaseLineVersionNumber;
                                    _sqlRunner.RunUpgrade(dbv.CompileSql(prefix, SqlStatementType.PreDeployment, includeHistory), connectionString);
                                    _sqlRunner.RunUpgrade(dbv.CompileSql(prefix, SqlStatementType.PostDeployment, includeHistory), connectionString);
                                });

                var lastVersion = databaseVersions.LastOrDefault();
                if (lastVersion != null)
                {
                    bool includeHistory = lastVersion.VersionNumber >= database.BaseLineVersionNumber;
                    var compiledSqlPre = lastVersion.CompileSql(prefix, SqlStatementType.PreDeployment, includeHistory);
                    var compiledSqlPost = lastVersion.CompileSql(prefix, SqlStatementType.PostDeployment, includeHistory);

                    _sqlRunner.RunRollback(compiledSqlPost, connectionString);
                    _sqlRunner.RunRollback(compiledSqlPre, connectionString);

                    _sqlRunner.RunUpgrade(compiledSqlPre, connectionString);
                    _sqlRunner.RunUpgrade(compiledSqlPost, connectionString);
                }

                return new DatabaseBuildResult(true, BUILD_SUCCESS_MESSAGE);
            }
            catch(SqlCommandException ex)
            {
                string message = string.Format(BUILD_FAILED_TEMPLATE, ex.Message, ex.FailureText, ex.ConnectionString);
                return new DatabaseBuildResult(false, message);
            }
        }
    }
}