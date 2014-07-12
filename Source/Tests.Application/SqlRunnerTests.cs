using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DBTM.Application.Factories;
using DBTM.Application.SQL;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application
{
    [TestFixture]
    public class SqlRunnerTests
    {
        private IDbConnection _dbConnection;
        private IDbCommandFactory _dbCommandFactory;
        private IDbCommand _command;
        private ISqlRunner _runner;
        private IDatabaseConnectionFactory _connectionFactory;

        [SetUp]
        public void SetUp()
        {
            _dbConnection = MockRepository.GenerateMock<IDbConnection>();
            _dbCommandFactory = MockRepository.GenerateMock<IDbCommandFactory>();
            _command = MockRepository.GenerateMock<IDbCommand>();
            _connectionFactory = MockRepository.GenerateMock<IDatabaseConnectionFactory>();

            _runner = new SqlRunner(_dbCommandFactory, _connectionFactory);
        }

        [TearDown]
        public void TearDown()
        {
            _dbConnection.VerifyAllExpectations();
            _dbCommandFactory.VerifyAllExpectations();
            _command.VerifyAllExpectations();
            _connectionFactory.VerifyAllExpectations();
        }

        [Test]
        public void RunExecutesStatementsAgainstNonAdminConnectionString()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";
            var statement2 = "statement2";
            var statement3 = "statement3";

            var compiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            compiledSql.AddUpgrade(new TestableCompiledUpgradeSql(statement1));
            compiledSql.AddUpgrade(new TestableCompiledUpgradeSql(statement2));
            compiledSql.AddUpgrade(new TestableCompiledUpgradeSql(statement3));
            compiledSql.AddRollback(new TestableRollbackUpgradeSql("rollback stuff"));

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _command.Expect(c => c.Connection = _dbConnection).Repeat.Times(3);
            _command.Expect(c => c.ExecuteNonQuery()).Return(1).Repeat.Times(3);

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _dbCommandFactory.Expect(cf => cf.Create(statement2)).Return(_command);
            _dbCommandFactory.Expect(cf => cf.Create(statement3)).Return(_command);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            _runner.RunUpgrade(compiledSql, connectionString);

        }

        [Test]
        public void RunUpgradeExecutesStatementsAgainstNonAdminConnectionStringForBasicCompiledSql()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";

            var compiledSql = new CompiledSql(statement1, "rollback");

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _command.Expect(c => c.Connection = _dbConnection).Repeat.Times(1);
            _command.Expect(c => c.ExecuteNonQuery()).Return(1).Repeat.Times(1);

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            _runner.RunUpgrade(compiledSql, connectionString);

        }


        [Test]
        public void RunRollbackExecutesStatementsAgainstNonAdminConnectionStringForBasicCompiledSql()
        {
            string connectionString = "some connection string";

            var compiledSql = new CompiledSql("statement1", "rollback");

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _command.Expect(c => c.Connection = _dbConnection).Repeat.Times(1);
            _command.Expect(c => c.ExecuteNonQuery()).Return(1).Repeat.Times(1);

            _dbCommandFactory.Expect(cf => cf.Create("rollback")).Return(_command);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            _runner.RunRollback(compiledSql, connectionString);

        }

        [Test]
        public void RollbackExecutesStatementsAgainstNonAdminConnectionString()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";
            var statement2 = "statement2";
            var statement3 = "statement3";

            var compiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            compiledSql.AddRollback(new TestableRollbackUpgradeSql(statement1));
            compiledSql.AddRollback(new TestableRollbackUpgradeSql(statement2));
            compiledSql.AddRollback(new TestableRollbackUpgradeSql(statement3));
            compiledSql.AddUpgrade(new TestableCompiledUpgradeSql("rollback stuff"));

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _dbCommandFactory.Expect(cf => cf.Create(statement2)).Return(_command);
            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _dbCommandFactory.Expect(cf => cf.Create(statement3)).Return(_command);

            _command.Expect(c => c.Connection = _dbConnection).Repeat.Times(3);
            _command.Expect(c => c.ExecuteNonQuery()).Return(1).Repeat.Times(3);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            _runner.RunRollback(compiledSql, connectionString);
        }


        [Test]
        public void RunThrowsBuildFailedExceptionWithConnectionStringAndCommandTextInfoIfSqlExceptionOccurs()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _command.Expect(c => c.Connection = _dbConnection);
            _command.Expect(c => c.ExecuteNonQuery()).Throw(new Exception());

            _dbConnection.Expect(dc => dc.ConnectionString).Return(connectionString);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            try
            {
                var compiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
                compiledSql.AddUpgrade(new TestableCompiledUpgradeSql(statement1));
                _runner.RunUpgrade(compiledSql, connectionString);
            }
            catch (SqlCommandException ex)
            {
                Assert.AreEqual(connectionString, ex.ConnectionString);
                Assert.AreEqual(statement1, ex.FailureText);
            }
        }

        [Test]
        public void RunThrowsBuildFailedExceptionWithConnectionStringAndCommandTextInfoIfSqlExceptionOccursRollback()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _command.Expect(c => c.Connection = _dbConnection);
            _command.Expect(c => c.ExecuteNonQuery()).Throw(new Exception());

            _dbConnection.Expect(dc => dc.ConnectionString).Return(connectionString);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            try
            {
                var compiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
                compiledSql.AddRollback(new TestableRollbackUpgradeSql(statement1));
                _runner.RunRollback(compiledSql, connectionString);
            }
            catch (SqlCommandException ex)
            {
                Assert.AreEqual(connectionString, ex.ConnectionString);
                Assert.AreEqual(statement1, ex.FailureText);
            }
        }

        [Test]
        public void RunAdminScriptsUsesProvidedConnectionString()
        {
            string connectionString = "some connection string";

            var statement1 = "statement1";
            var statement2 = "statement2";
            var statement3 = "statement3";

            List<string> sqlStatements = new List<string>
                                    {
                                        statement1,
                                        statement2,
                                        statement3,
                                    };


            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _command.Expect(c => c.Connection = _dbConnection).Repeat.Times(3);
            _command.Expect(c => c.ExecuteNonQuery()).Return(1).Repeat.Times(3);

            _dbCommandFactory.Expect(cf => cf.Create(statement2)).Return(_command);
            _dbCommandFactory.Expect(cf => cf.Create(statement3)).Return(_command);

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            _runner.RunAdminScripts(sqlStatements, connectionString);
        }
        [Test]
        public void RunAdminScriptThrowsDirectoryNotFoundWhenSqlExceptionIndicatesDirectoryNotFound()
        {

            string connectionString = "some connection string";

            var statement1 = "statement1";

            List<string> sqlStatements = new List<string> { statement1 };

            _connectionFactory.Expect(cf => cf.Create(connectionString)).Return(_dbConnection);

            _dbConnection.Expect(d => d.Open());

            _dbCommandFactory.Expect(cf => cf.Create(statement1)).Return(_command);
            _command.Expect(c => c.Connection = _dbConnection);
            _command.Expect(c => c.ExecuteNonQuery()).Throw(new Exception("Directory lookup for the file \"C:\\databases\\foo\\Database2_Data.mdf\" failed with the operating system error 2(The system cannot find the file specified.).\r\nCREATE DATABASE failed. Some file names listed could not be created. Check related errors...."));

            _dbConnection.Expect(d => d.Close());
            _dbConnection.Expect(d => d.Dispose());

            Assert.Throws<SqlCommandDirectoryNotFoundException>(() => _runner.RunAdminScripts(sqlStatements, connectionString));
        }
    }


    public class TestableCompiledUpgradeSql : CompiledUpgradeSql
    {
        private readonly string _sql;

        public TestableCompiledUpgradeSql(string sql) :
            base(sql, "", Guid.Empty, SqlStatementType.PreDeployment, 0, false)
        {
            _sql = sql;
        }

        public override string ToString()
        {
            return _sql;
        }
    }

    public class TestableRollbackUpgradeSql : CompiledRollbackSql
    {
        private readonly string _sql;

        public TestableRollbackUpgradeSql(string sql) :
            base(sql, "", Guid.Empty, SqlStatementType.PreDeployment, 0, false)
        {
            _sql = sql;
        }

        public override string ToString()
        {
            return _sql;
        }
    }

}