using DBTM.Application;
using DBTM.Cmd;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd
{
    [TestFixture]
    public class FullBuildApplicationRunnerTests
    {
        private FullBuildApplicationRunner _runner;
        private IDatabaseRepository _databaseRepository;
        private IDatabaseBuildService _databaseBuildService;
        private ISqlServerDatabaseSettings _databaseSettings;
        private ISqlServerDatabaseSettingsBuilder _sqlServerSettingsBuilder;
        private IFullBuildArguments _fullBuildArguments;

        private string _databaseFilePath = "blah";
        private string _databaseName = "blah";
        private string _server = "localhost";
        private string _userName = "user";
        private string _password = "pwd";
        private string _connectionString = "Data Source={0};Initial Catalog={1};User Id={2};Password={3};Connection Timeout=240";

        [SetUp]
        public void Setup()
        {
            _databaseRepository = MockRepository.GenerateMock<IDatabaseRepository>();
            _databaseBuildService = MockRepository.GenerateMock<IDatabaseBuildService>();
            _sqlServerSettingsBuilder = MockRepository.GenerateMock<ISqlServerDatabaseSettingsBuilder>();

            _databaseSettings = MockRepository.GenerateStub<ISqlServerDatabaseSettings>();

            // stub wasnt working so I made a real
            _fullBuildArguments =
                new FullBuildArguments(new[]
                                  {
                                      "-databaseName=" + _databaseName,
                                      "-server=" + _server,
                                      "-userName=" + _userName,
                                      "-password=" + _password,
                                      @"-dataFilePath=-c:\somewhere\",
                                      "-databaseFilePath=" + _databaseFilePath
                                  });

            _runner = new FullBuildApplicationRunner(_databaseRepository, _databaseBuildService, _sqlServerSettingsBuilder);
        }

        [TearDown]
        public void TearDown()
        {
            _databaseRepository.VerifyAllExpectations();
            _databaseBuildService.VerifyAllExpectations();
            _sqlServerSettingsBuilder.VerifyAllExpectations();
        }

        [Test]
        public void RunCreatesInnerContainerResolvesLoadsDatabaseFromRepositoryAndCallsDatabaseBuildServiceToBuildTheDatabase()
        {
           
            Database database = new Database(_databaseName);
            ApplicationRunnerResult result = new ApplicationRunnerResult(true, "message");
            string connectionString = string.Format(_connectionString, _server, _databaseName, _userName, _password);
            
            _sqlServerSettingsBuilder.Expect(sb => sb.Build(_fullBuildArguments)).Return(_databaseSettings);
            _databaseSettings.Stub(s => s.ConnectionString).Return(connectionString);

            _databaseRepository.Expect(dr => dr.Load(_databaseFilePath)).Return(database);
            _databaseBuildService.Expect(dbs => dbs.FullBuild(database, _databaseSettings)).Return(new DatabaseBuildResult(true,"message"));

            var actual = _runner.Run(_fullBuildArguments);

            Assert.AreEqual(result, actual);
        }
    }
}