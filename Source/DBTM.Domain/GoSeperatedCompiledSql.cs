using System;
using System.Collections.Generic;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public class GoSeperatedCompiledSql :ICompiledSql
    {
        private readonly string _update;
        private readonly string _rollback;

        public GoSeperatedCompiledSql(string update, string rollback)
        {
            _update = update;
            _rollback = rollback;
        }

        public string Upgrade
        {
            get { return _update; }
        }

        public string Rollback
        {
            get { return _rollback; }
        }

        public IEnumerable<string> UpgradeStatements
        {
            get { return _update.Split(new[] {"go", "GO", "gO", "Go"}, StringSplitOptions.RemoveEmptyEntries); }
        }

        public IEnumerable<string> RollbackStatements
        {
            get { return _rollback.Split(new[] { "go", "GO", "gO", "Go" }, StringSplitOptions.RemoveEmptyEntries); }
        }
    }
}