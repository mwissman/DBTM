using System;
using System.Data.SqlClient;

namespace DBTM.Application
{
    public class SqlServerConnectionStringTester : ITestSqlServerConnectionStrings
    {
        public bool IsValid(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}