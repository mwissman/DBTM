using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTM.Domain.Entities
{
    public class CompiledVersionSql : ICompiledSql
    {
        public virtual string Upgrade
        {
            get
            {
                var upgradeSqlStringBuilder = new StringBuilder();
                upgradeSqlStringBuilder.AppendLine(string.Format("-- Version {0} - {1} ", _versionNumber, _sqlStatementType).PadRight(65, '-'));


                foreach (var statement in _upgradeCompiledSqls)
                {
                    upgradeSqlStringBuilder.AppendLine();
                    upgradeSqlStringBuilder.Append(statement);
                    upgradeSqlStringBuilder.AppendLine("GO");
                    upgradeSqlStringBuilder.AppendLine();
                    upgradeSqlStringBuilder.AppendLine("----------------------------------------------------------------");
                }

                return upgradeSqlStringBuilder.ToString();
            }
        }

        public virtual string Rollback
        {
            get
            {
                var rollbackSqlStringBuilder = new StringBuilder();
                rollbackSqlStringBuilder.AppendLine(string.Format("-- Version {0} - {1} ", _versionNumber, _sqlStatementType).PadRight(65, '-'));

                foreach (var statement in _rollbackCompiledSql)
                {
                    rollbackSqlStringBuilder.AppendLine();
                    rollbackSqlStringBuilder.Append(statement);
                    rollbackSqlStringBuilder.AppendLine("GO");
                    rollbackSqlStringBuilder.AppendLine();

                    rollbackSqlStringBuilder.AppendLine("----------------------------------------------------------------");
                }

                return rollbackSqlStringBuilder.ToString();
            }
        }

        public CompiledVersionSql(int versionNumber, SqlStatementType sqlStatementType)
        {
            _versionNumber = versionNumber;
            _sqlStatementType = sqlStatementType;
        }

        private readonly IList<CompiledUpgradeSql> _upgradeCompiledSqls = new List<CompiledUpgradeSql>();
        private readonly IList<CompiledRollbackSql> _rollbackCompiledSql = new List<CompiledRollbackSql>();

        private readonly int _versionNumber;
        private readonly SqlStatementType _sqlStatementType;

        public IEnumerable<string> UpgradeStatements
        {
            get { return _upgradeCompiledSqls.Select(s=>s.ToString()); }
        }

        public IEnumerable<string> RollbackStatements
        {
            get { return _rollbackCompiledSql.Select(s => s.ToString()); }
        }

        public void AddUpgrade(CompiledUpgradeSql compiledUpgradeSql)
        {
            _upgradeCompiledSqls.Add(compiledUpgradeSql);
        }

        public void AddRollback(CompiledRollbackSql compiledRollbackSql)
        {
            _rollbackCompiledSql.Add(compiledRollbackSql);
        }
    }
}