using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public interface IMigrator
    {
        void Migrate(Database database);
    }
}