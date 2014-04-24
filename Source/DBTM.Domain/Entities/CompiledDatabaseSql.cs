using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTM.Domain.Entities
{
    public class CompiledDatabaseSql : ICompiledSql
    {
        private readonly string _dbName;
        private readonly ICompiledSql _compiledHistorySql;

        public virtual string Upgrade
        {
            get
            {
                var upgradeSqlStringBuilder = new StringBuilder();
               
                upgradeSqlStringBuilder.AppendLine("".PadRight(65, '-'));
                upgradeSqlStringBuilder.AppendLine(string.Format("-- {0}", _dbName));
                upgradeSqlStringBuilder.AppendLine();
                upgradeSqlStringBuilder.AppendLine(_compiledHistorySql.Upgrade);
                upgradeSqlStringBuilder.AppendLine();

                foreach (var version in _compiledVersionSqls)
                {
                    upgradeSqlStringBuilder.AppendLine(string.Format("{0}", version.Pre.Upgrade));
                    upgradeSqlStringBuilder.AppendLine(string.Format("{0}", version.Post.Upgrade));
                }

                return upgradeSqlStringBuilder.ToString();
            }
        }

        public virtual string Rollback
        {
            get
            {
                var rollbackSqlStringBuilder = new StringBuilder();

                rollbackSqlStringBuilder.AppendLine("".PadRight(65, '-'));
                rollbackSqlStringBuilder.AppendLine(string.Format("-- {0}", _dbName));
                rollbackSqlStringBuilder.AppendLine();

                foreach (var version in _compiledVersionSqls.Reverse())
                {
                    rollbackSqlStringBuilder.AppendLine(string.Format("{0}", version.Post.Rollback));
                    rollbackSqlStringBuilder.AppendLine(string.Format("{0}", version.Pre.Rollback));
                }

                rollbackSqlStringBuilder.AppendLine();
                rollbackSqlStringBuilder.AppendLine(_compiledHistorySql.Rollback);
                rollbackSqlStringBuilder.AppendLine();

                return rollbackSqlStringBuilder.ToString();
            }
        }

        public IEnumerable<string> UpgradeStatements
        {
            get
            {
                var updateStatements=new List<string>();

                updateStatements.AddRange(_compiledHistorySql.UpgradeStatements);

                foreach (var version in _compiledVersionSqls)
                {
                    updateStatements.AddRange(version.Pre.UpgradeStatements);
                    updateStatements.AddRange(version.Post.UpgradeStatements);
                }

                return updateStatements;
            }
        }

        public IEnumerable<string> RollbackStatements
        {
            get
            {
                var rollbackStatements = new List<string>();

                foreach (var version in _compiledVersionSqls.Reverse())
                {
                    rollbackStatements.AddRange(version.Post.RollbackStatements);
                    rollbackStatements.AddRange(version.Pre.RollbackStatements);
                }
                rollbackStatements.AddRange(_compiledHistorySql.RollbackStatements);

                return rollbackStatements;
            }
        }

        public CompiledDatabaseSql(string dbName, ICompiledSql compiledHistorySql)
        {
            _dbName = dbName;
            _compiledHistorySql = compiledHistorySql;
        }

        private readonly IList<CompiledVersion> _compiledVersionSqls = new List<CompiledVersion>();

        public void AddVersion(ICompiledSql preDeployCompiledVersionSql, ICompiledSql postDeployCompiledSql)
        {
            _compiledVersionSqls.Add(new CompiledVersion(preDeployCompiledVersionSql, postDeployCompiledSql));
        }

        private class CompiledVersion
        {
            private readonly ICompiledSql _preCompiledVersion;
            private readonly ICompiledSql _postCompiledVerison;

            public CompiledVersion(ICompiledSql preCompiledVersion, ICompiledSql postCompiledVerison)
            {
                _preCompiledVersion = preCompiledVersion;
                _postCompiledVerison = postCompiledVerison;
            }

            public ICompiledSql Pre { get { return _preCompiledVersion; } }
            public ICompiledSql Post { get { return _postCompiledVerison; } }
        }

    }
}