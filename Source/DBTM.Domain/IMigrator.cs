using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public interface IMigrator
    {
        void EnsureStatementsHaveIds(Database database);
    }
}