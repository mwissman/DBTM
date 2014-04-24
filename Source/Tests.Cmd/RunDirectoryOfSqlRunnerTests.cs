using System.Collections.Generic;
using DBTM.Application;
using DBTM.Application.SQL;
using DBTM.Cmd;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd
{
    [TestFixture]
    public class RunDirectoryOfSqlRunnerTests
    {
        private const string _connectionstring = "ConnectionString";
        private RunDirectoryOfSqlRunner _runDirectoryOfSqlRunner;
        private IRunDirectoryOfSqlArguments _arguments;
        private ISqlRunner _sqlRunner;
        private ISqlFileReader _sqlFileReader;
        private string _directoryPath;
        private ISqlFileList _sqlFileList;
        private ISqlServerDatabaseSettingsBuilder _sqlServerDatabaseSettingsBuilder;
        private ISqlServerDatabaseSettings _sqlServerDatabaseSettings;
        private List<string> _files = new List<string>();

        [SetUp]
        public void Setup()
        {
            _sqlRunner = MockRepository.GenerateMock<ISqlRunner>();
            _sqlFileReader = MockRepository.GenerateMock<ISqlFileReader>();
            _sqlServerDatabaseSettingsBuilder = MockRepository.GenerateStub<ISqlServerDatabaseSettingsBuilder>();
            _sqlServerDatabaseSettings = MockRepository.GenerateStub<ISqlServerDatabaseSettings>();
            _sqlServerDatabaseSettings.Stub(x => x.ConnectionString).Return(_connectionstring);
            _sqlServerDatabaseSettingsBuilder
                .Stub(x => x.Build(Arg<IFullBuildArguments>.Is.Anything))
                .Return(_sqlServerDatabaseSettings);

            _arguments = MockRepository.GenerateStub<IRunDirectoryOfSqlArguments>();
            _arguments.Stub(x => x.HasRequiredArguments).Return(true);
            _directoryPath = @"C:\redbox\something";
            _arguments.Stub(x => x.ScriptDirectoryPath).Return(_directoryPath);


            _runDirectoryOfSqlRunner = new RunDirectoryOfSqlRunner(_sqlRunner, _sqlServerDatabaseSettingsBuilder, _sqlFileReader);
        }

        [TearDown]
        public void TearDown()
        {
            _sqlRunner.VerifyAllExpectations();
            _sqlFileReader.VerifyAllExpectations();
        }

        [Test]
        public void RunSqlFilesInAlphabeticalOrder()
        {
            _sqlFileList = MockRepository.GenerateMock<ISqlFileList>();

            _sqlFileReader
                .Expect(x => x.GetFromDirectoryPath(_directoryPath))
                .Return(_sqlFileList);

            var firstFile = MockRepository.GenerateStub<ISqlFile>();
            firstFile.Stub(x => x.Contents).Return("contents 1");

            var secondFile = MockRepository.GenerateStub<ISqlFile>();
            secondFile.Stub(x => x.Contents).Return("contents 2");

            var sqlFiles = new List<ISqlFile>
                               {
                                   firstFile,
                                   secondFile
                               };

            _sqlFileList
                .Expect(x => x.Files)
                .Return(sqlFiles);



            var result = _runDirectoryOfSqlRunner.Run(_arguments);

            Assert.IsTrue(result.Succeeded);


            _sqlRunner.AssertWasCalled(
                x =>
                x.RunAdminScripts(Arg<IList<string>>.Matches(y => RunsScriptInOrder(y)),
                                  Arg<string>.Is.Equal(_connectionstring)));
            
            _sqlRunner.AssertWasCalled(
                x =>
                x.RunAdminScripts(Arg<IList<string>>.Matches(y => RunsScriptInOrder(y)),
                                  Arg<string>.Is.Equal(_connectionstring)));

            CollectionAssert.Contains(_files, "contents 1");
            CollectionAssert.Contains(_files, "contents 2");
        }

        private bool RunsScriptInOrder(IList<string> list)
        {
            _files.AddRange(list);
           
            return true;
        }
    }
}