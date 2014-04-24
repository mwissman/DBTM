using System;
using System.Text;

namespace DBTM.Domain.Entities
{
    public class CompiledRollbackSql : CompiledSqlStatementBase
    {
        private readonly string _rollbackSql;
        private readonly string _databaseNamePrefix;
        private readonly int _versionNumber;
        private readonly bool _includeHistoryStatements;
        private readonly Guid _statementId;
        private readonly SqlStatementType _statementType;
      
        public CompiledRollbackSql(string rollbackSql, string databaseNamePrefix, Guid statementId, SqlStatementType statementType, int versionNumber, bool includeHistoryStatements)
        {
            _rollbackSql = rollbackSql;
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
                sqlBuilder.AppendFormat("   EXEC ('{0}');\r\n", ProcessCrossDatabasePrefix(_rollbackSql, _databaseNamePrefix).Replace("'", "''"));
                sqlBuilder.AppendLine();
                sqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Rollback';\r\n", _versionNumber,_statementId, _statementType.ToString());
            }
            else
            {
                sqlBuilder.AppendFormat("{0}\r\n", ProcessCrossDatabasePrefix(_rollbackSql, _databaseNamePrefix));
            }

            return sqlBuilder.ToString();
        }
    }
}