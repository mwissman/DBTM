using DBTM.Domain.Entities;

namespace DBTM.Application
{
    public interface IDatabaseBuildService
    {
        DatabaseBuildResult FullBuild(Database database, ISqlServerDatabaseSettings sqlServerDatabaseSettings);
    }
}