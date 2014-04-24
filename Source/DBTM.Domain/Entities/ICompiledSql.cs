using System.Collections.Generic;

namespace DBTM.Domain.Entities
{
    public interface ICompiledSql
    {
        string Upgrade { get; }
        string Rollback { get; }

        IEnumerable<string> UpgradeStatements { get; } 
        IEnumerable<string> RollbackStatements { get; } 
    }
}