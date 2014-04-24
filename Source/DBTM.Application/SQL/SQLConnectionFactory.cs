using System.Data;
using System.Data.SqlClient;
using DBTM.Application.Factories;

namespace DBTM.Application.SQL
{
    public class SQLConnectionFactory : IDatabaseConnectionFactory
    {
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}