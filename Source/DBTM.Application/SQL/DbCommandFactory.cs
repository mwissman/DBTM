using System.Data;
using System.Data.SqlClient;
using DBTM.Application.Factories;

namespace DBTM.Application.SQL
{
    public class DbCommandFactory : IDbCommandFactory
    {
        public IDbCommand Create(string sql)
        {
            return new SqlCommand(sql);
        }
    }
}