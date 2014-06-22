using System.IO;
using System.Linq;
using DBTM.Domain.Entities;

namespace DBTM.Application
{
    public class Compiler : ICompiler
    {
        private readonly ISqlFileWriter _sqlFileWriter;
        private readonly ISqlScriptRepository _sqlScriptRepository;

        public Compiler(ISqlFileWriter sqlFileWriter, ISqlScriptRepository sqlScriptRepository)
        {
            _sqlFileWriter = sqlFileWriter;
            _sqlScriptRepository = sqlScriptRepository;
        }

        public void CompileLatestVersion(Database database, string compiledSqlFolderPath, string databasePrefix)
        {
            int maxVersionNumber = database.Versions.Max(v => v.VersionNumber);

            bool includeHistoryStatements = maxVersionNumber >= database.BaseLineVersionNumber;

            var maxVersion = database.Versions.First(v => v.VersionNumber == maxVersionNumber);

            if (!maxVersion.HasStatements)
            {
                return;
            }

            if (maxVersion.IsBaseline)
            {
                _sqlFileWriter.Write(_sqlScriptRepository.LoadHistroySql(),
                                     Path.Combine(compiledSqlFolderPath, "0_DBTMHistoryUpgrade.sql"),
                                     Path.Combine(compiledSqlFolderPath, "0_DBTMHistoryRollback.sql"));
            }

            CompileVersion(maxVersion, compiledSqlFolderPath, databasePrefix, includeHistoryStatements);
        }

        public void CompileAllVersions(Database database, string compiledSqlFolderPath)
        {
            var historySql=_sqlScriptRepository.LoadHistroySql();
            _sqlFileWriter.Write(database.CompileAllVersions(historySql),
                                 Path.Combine(compiledSqlFolderPath,
                                              string.Format("{0}{1}{2}_AllVersionsUpgrade.sql", compiledSqlFolderPath,
                                                            Path.DirectorySeparatorChar, database.DbName)),
                                 Path.Combine(compiledSqlFolderPath,
                                              string.Format("{0}{1}{2}_AllVersionsRollback.sql", compiledSqlFolderPath,
                                                            Path.DirectorySeparatorChar, database.DbName)));
        }

        private void CompileVersion(DatabaseVersion version, string compileScriptPath, string databasePrefix,
                                    bool includeHistoryStatements)
        {
            CompileStatements(version,
                              version.PreDeploymentStatements,
                              compileScriptPath,
                              databasePrefix,
                              SqlStatementType.PreDeployment,
                              "PreDeploymentUpgrade.sql",
                              "PreDeploymentRollback.sql",
                              includeHistoryStatements);

            CompileStatements(version,
                              version.PostDeploymentStatements,
                              compileScriptPath,
                              databasePrefix,
                              SqlStatementType.PostDeployment,
                              "PostDeploymentUpgrade.sql",
                              "PostDeploymentRollback.sql",
                              includeHistoryStatements);
        }

        private void CompileStatements(DatabaseVersion version,
                                       SqlStatementCollection statements,
                                       string compileScriptPath,
                                       string prefix,
                                       SqlStatementType sqlStatementType,
                                       string upgradeFileName,
                                       string rollbackFileName,
                                       bool includeHistoryStatements)
        {
            if (statements.Count > 0)
            {
                _sqlFileWriter.Write(version.CompileSql(prefix,
                                                        sqlStatementType,
                                                        includeHistoryStatements),
                                     Path.Combine(compileScriptPath, upgradeFileName),
                                     Path.Combine(compileScriptPath, rollbackFileName));
            }
        }
    }
}