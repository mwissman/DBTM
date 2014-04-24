using System.Data;

namespace DBTM.Application.Factories
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection Create(string connectionString);
    }
}