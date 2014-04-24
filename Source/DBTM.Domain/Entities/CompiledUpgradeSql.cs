using System;
using System.Text;

namespace DBTM.Domain.Entities
{
    public class CompiledUpgradeSql : CompiledSqlStatementBase
    {
        private readonly string _sql;
        private readonly string _databaseNamePrefix;
        private readonly Guid _statementId;
        private readonly SqlStatementType _statementType;
        private readonly int _versionNumber;
        private readonly bool _includeHistoryStatements;

        public CompiledUpgradeSql(string sql, string databaseNamePrefix, Guid statementId, SqlStatementType statementType, int versionNumber, bool includeHistoryStatements)
        {
            _sql = sql;
            _databaseNamePrefix = databaseNamePrefix;
            _statementId = statementId;
            _statementType = statementType;
            _versionNumber = versionNumber;
            _includeHistoryStatements = includeHistoryStatements;
        }

        public override string ToString()
        {
            var sqlBuilder = new StringBuilder();
            if (_includeHistoryStatements)
            {
                if (_statementType == SqlStatementType.PostDeployment || _statementType == SqlStatementType.PreDeployment)
                {
                    sqlBuilder.AppendLine("DECLARE @canrun BIT = 0;");
                    sqlBuilder.AppendFormat("EXEC @canrun=[DBTM].sp_CanRunStatementUpgrade '{0}';\r\n", _statementId);
                    sqlBuilder.AppendLine("IF (@canrun)=1");
                    sqlBuilder.AppendLine("BEGIN");
                    sqlBuilder.AppendFormat("    PRINT 'Running Statement {0}.';\r\n", _statementId);
                    sqlBuilder.AppendLine();
                    sqlBuilder.AppendFormat("    EXEC ('{0}');\r\n", ProcessCrossDatabasePrefix(_sql, _databaseNamePrefix).Replace("'", "''"));
                    sqlBuilder.AppendLine();
                    sqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Upgrade';\r\n", _versionNumber, _statementId, _statementType.ToString());
                    sqlBuilder.AppendLine("END");
                    sqlBuilder.AppendLine("ELSE");
                    sqlBuilder.AppendLine("BEGIN");
                    sqlBuilder.AppendFormat("	PRINT 'Statement {0} has already been executed.';\r\n", _statementId);
                    sqlBuilder.AppendLine("END");
                }
                else
                {
                    sqlBuilder.AppendFormat("    EXEC ('{0}');\r\n", ProcessCrossDatabasePrefix(_sql, _databaseNamePrefix).Replace("'", "''"));
                    sqlBuilder.AppendLine();
                    sqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Upgrade';\r\n", _versionNumber, _statementId, _statementType.ToString());
                }
            }
            else
            {
                sqlBuilder.AppendFormat("{0}\r\n", ProcessCrossDatabasePrefix(_sql, _databaseNamePrefix));
            }

            return sqlBuilder.ToString();
        }
    }
}