using System;
using DBTM.Application;
using DBTM.Application.SQL;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application
{
    [TestFixture]
    public class DatabaseBuildServiceTests
    {
        private readonly DateTime _now = DateTime.Now;
        private string _connectionString;
        private string _databaseNamePrefix;

        private ISqlRunner _sqlRunner;
        private ISqlServerDatabase _sqlServerDatabase;
        private ISqlServerDatabaseSettings _sqlServerDatabaseSettings;
        private IDatabaseBuildService _databaseBuildService;
        private ISqlScriptRepository _scriptRepository;

        [SetUp]
        public void Setup()
        {
            _connectionString = "connection string";
            _databaseNamePrefix = "databasePrefix_";


            _sqlRunner = MockRepository.GenerateMock<ISqlRunner>();
            _sqlServerDatabase = MockRepository.GenerateMock<ISqlServerDatabase>();
            _sqlServerDatabaseSettings = MockRepository.GenerateStub<ISqlServerDatabaseSettings>();
            _scriptRepository = MockRepository.GenerateMock<ISqlScriptRepository>();

            _databaseBuildService = new DatabaseBuildService(_sqlRunner, _sqlServerDatabase, _scriptRepository);
        }

        [TearDown]
        public void TearDown()
        {
            _sqlRunner.VerifyAllExpectations();
            _sqlServerDatabase.VerifyAllExpectations();
            _scriptRepository.VerifyAllExpectations();
        }

        [Test]
        public void BuildReturnsFailureResultIfSqlCommandExceptionOccurs()
        {
            string commandText = "command text";
            string exceptionMessage = "some exception message";
            CompiledSql compiledHistorySql = new CompiledSql("", "");

            string expectedResultMessage = string.Format(
                    "Build Failed!\n\nSql Server Message:\n{0}\n\nCommand Text:\n{1}\n\nConnection String:\n{2}\n",
                    exceptionMessage,
                    commandText,
                    _connectionString);

            var commandException = new SqlCommandException(commandText, _connectionString, exceptionMessage, new Exception());

            var compiledSqlVersion1Pre = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            var compiledSqlVersion1Post = new CompiledVersionSql(0, SqlStatementType.PreDeployment);


            var databaseVersion1 = MockRepository.GenerateStub<DatabaseVersion>(1, _now);
            databaseVersion1.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PreDeployment, false)).Return(compiledSqlVersion1Pre);
            databaseVersion1.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PostDeployment, false)).Return(compiledSqlVersion1Post);
           
            Database database = new Database("name");
            database.Versions.Add(databaseVersion1);


            _sqlServerDatabase.Expect(d => d.Initialize(_sqlServerDatabaseSettings));

            _sqlServerDatabaseSettings.Stub(s => s.ConnectionString).Return(_connectionString);
            _sqlServerDatabaseSettings.Stub(s => s.CrossDatabaseNamePrefix).Return(_databaseNamePrefix);

            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledSqlVersion1Pre, _connectionString)).Throw(commandException);

            _scriptRepository.Expect(r => r.LoadHistroySql()).Return(compiledHistorySql);

            var result = _databaseBuildService.FullBuild(database, _sqlServerDatabaseSettings);

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(expectedResultMessage, result.Message);
        }

        [Test]
        public void BuildDoesNothingIfThereAreNoVersions()
        {
            var compiledHistorySql = new CompiledSql("", "");
            var versions = new DatabaseVersionCollection();

            var database = new Database("name") { Versions = versions };

            _sqlServerDatabaseSettings.Stub(s => s.ConnectionString).Return(_connectionString);
            _sqlServerDatabaseSettings.Stub(s => s.CrossDatabaseNamePrefix).Return(_databaseNamePrefix);

            _scriptRepository.Expect(r => r.LoadHistroySql()).Return(compiledHistorySql);
            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledHistorySql, _connectionString));

            var result = _databaseBuildService.FullBuild(database, _sqlServerDatabaseSettings);

            Assert.IsTrue(result.Succeeded);

            _sqlServerDatabase.AssertWasNotCalled(sd => sd.Initialize(Arg<ISqlServerDatabaseSettings>.Is.Anything));
           
        }

        [Test]
        public void BuildRunsPreDeploymentScriptsThenPostDeploymentScriptsForEachVersionRollsbackLastVersionAndUpgradesAgainToTestRollbackScriptsAndIncludesHistoryAtBaseLineVersion()
        {
            var compiledHistorySql = new CompiledSql("", "");
            var compiledSqlVersion2Pre = new CompiledVersionSql(0,SqlStatementType.PreDeployment);
            var compiledSqlVersion2Post = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            var compiledSqlVersion1Pre = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            var compiledSqlVersion1Post = new CompiledVersionSql(0, SqlStatementType.PreDeployment);

            var databaseVersion1 = MockRepository.GenerateStub<DatabaseVersion>(1, _now);

            databaseVersion1.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PreDeployment, false)).Return(compiledSqlVersion1Pre);
            databaseVersion1.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PostDeployment, false)).Return(compiledSqlVersion1Post);

            var databaseVersion2 = MockRepository.GenerateMock<DatabaseVersion>(3, _now);

            databaseVersion2.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PreDeployment, true)).Return(compiledSqlVersion2Pre);
            databaseVersion2.Expect(dv => dv.CompileSql(_databaseNamePrefix, SqlStatementType.PostDeployment, true)).Return(compiledSqlVersion2Post);
            databaseVersion2.Stub(dv => dv.IsBaseline).Return(true);

            var database = new Database("name");
            database.Versions.Add(databaseVersion1);
            database.Versions.Add(databaseVersion2);

            _sqlServerDatabase.Expect(d => d.Initialize(_sqlServerDatabaseSettings));

            _sqlServerDatabaseSettings.Stub(s => s.ConnectionString).Return(_connectionString);
            _sqlServerDatabaseSettings.Stub(s => s.CrossDatabaseNamePrefix).Return(_databaseNamePrefix);

            _scriptRepository.Expect(r => r.LoadHistroySql()).Return(compiledHistorySql);
            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledHistorySql, _connectionString));

            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledSqlVersion1Pre, _connectionString));
            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledSqlVersion1Post, _connectionString));

            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledSqlVersion2Pre, _connectionString)).Repeat.Twice();
            _sqlRunner.Expect(sr => sr.RunUpgrade(compiledSqlVersion2Post, _connectionString)).Repeat.Twice();

            _sqlRunner.Expect(sr => sr.RunRollback(compiledSqlVersion2Post, _connectionString));
            _sqlRunner.Expect(sr => sr.RunRollback(compiledSqlVersion2Pre, _connectionString));


            var result = _databaseBuildService.FullBuild(database, _sqlServerDatabaseSettings);

            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("Build Successful!", result.Message);

            databaseVersion2.VerifyAllExpectations();
            databaseVersion1.VerifyAllExpectations();
        }


    }
}