using System.Collections.Generic;

namespace DBTM.Domain.Entities
{
    public class CompiledSql : ICompiledSql
    {
        private readonly string _upgrade;
        private readonly string _rollback;

        public CompiledSql(string upgradeSql, string rollbackSql)
        {
            _upgrade = upgradeSql;
            _rollback = rollbackSql;
        }

        public string Upgrade
        {
            get { return _upgrade; }
        }

        public string Rollback
        {
            get { return _rollback; }
        }

        public IEnumerable<string> UpgradeStatements
        {
            get { return new[]{ _upgrade}; }
        }

        public IEnumerable<string> RollbackStatements
        {
            get { return new[] { _rollback }; }
        }
    }
}