using System;
using System.Collections.Generic;
using System.Data;
using DBTM.Application.Factories;
using DBTM.Domain.Entities;
using System.Linq;

namespace DBTM.Application.SQL
{
    public class SqlRunner : ISqlRunner
    {
        private readonly IDbCommandFactory _dbCommandFactory;
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public SqlRunner(IDbCommandFactory dbCommandFactory,
                         IDatabaseConnectionFactory connectionFactory)
        {
            _dbCommandFactory = dbCommandFactory;
            _connectionFactory = connectionFactory;
        }

        public void RunAdminScripts(IList<string> adminScripts, string adminConnectionString)
        {
            Run(adminScripts, adminConnectionString);
        }

        public void RunUpgrade(ICompiledSql compiledSql, string connectionString)
        {
            Run(compiledSql.UpgradeStatements, connectionString);
        }

        public void RunRollback(ICompiledSql compiledSql, string connectionString)
        {
            Run(compiledSql.RollbackStatements, connectionString);
        }

        private void Run(IEnumerable<string> sqlStatements, string connectionString)
        {
            using (var connection = _connectionFactory.Create(connectionString))
            {
                connection.Open();
                sqlStatements.ToList().ForEach(sql => RunSQLCommand(sql, connection));
                connection.Close();
            }
        }

        private void RunSQLCommand(string commandText, IDbConnection connection)
        {
            try
            {
                if (string.IsNullOrEmpty(commandText.Trim()))
                {
                    return;
                }

                var command = _dbCommandFactory.Create(commandText);
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string connectionString = connection.ConnectionString;
                connection.Close();
                connection.Dispose();

                if (ex.Message.StartsWith("Directory lookup for the file"))
                {
                    string missingDiretory = ex.Message.Split(new []{"\r\n"},StringSplitOptions.RemoveEmptyEntries)[0];
                    throw new SqlCommandDirectoryNotFoundException(missingDiretory);
                }

                throw new SqlCommandException(commandText, connectionString, ex.Message, ex);
            }
        }
    }
}