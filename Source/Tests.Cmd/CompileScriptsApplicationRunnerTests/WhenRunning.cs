using System;
using System.Windows.Input;
using DBTM.Application;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd.CompileScriptsApplicationRunnerTests
{
    public partial class CompileScriptsApplicationRunnerTests
    {
        [TestFixture]
        public class WhenRunning
        {
            private CompileScriptsApplicationRunner _runner;
            private ICompileScriptsArguments _arguments;
            private IDatabaseRepository _databaseRepository;
            private ICommand _compileVersionCommand;

            [SetUp]
            public void Setup()
            {
                ICompiler compiler = MockRepository.GenerateMock<ICompiler>();
                _arguments = MockRepository.GenerateStub<ICompileScriptsArguments>();

                _databaseRepository = MockRepository.GenerateMock<IDatabaseRepository>();
                _compileVersionCommand = MockRepository.GenerateMock<ICommand>();
               
                _runner = new CompileScriptsApplicationRunner(_databaseRepository,_compileVersionCommand);
            }

            [TearDown]
            public void TearDown()
            {
                _databaseRepository.VerifyAllExpectations();
                _compileVersionCommand.VerifyAllExpectations();
            }

            [Test]
            public void LoadsDatabaseSchemaExecutesCompileVersionCommandOnHighestVersionNumber()
            {
                Database database = MockRepository.GenerateStub<Database>((string)null);

                DatabaseVersion version1 = new DatabaseVersion(1, DateTime.Now);
                DatabaseVersion version2 = new DatabaseVersion(2, DateTime.Now);
                database.Versions=new DatabaseVersionCollection()
                                      {
                                          version1,
                                          version2
                                      };

                string databaseSchemaPath = "database schema path";

                _arguments.Stub(a => a.DatabaseSchemaFilePath).Return(databaseSchemaPath);

                _databaseRepository.Expect(r => r.Load(databaseSchemaPath)).Return(database);
                _compileVersionCommand.Expect(c => c.Execute(database));
                
                var result = _runner.Run(_arguments);

                Assert.AreEqual(true,result.Succeeded);

            }

            [Test]
            public void RunnerFailsWithErrorIfNoVersionsExist()
            {
                Database database = MockRepository.GenerateStub<Database>((string)null);

                string databaseSchemaPath = "database schema path";
                _arguments.Stub(a => a.DatabaseSchemaFilePath).Return(databaseSchemaPath);

                _databaseRepository.Expect(r => r.Load(databaseSchemaPath)).Return(database);
               
                var result = _runner.Run(_arguments);

                Assert.IsFalse(result.Succeeded);
                Assert.IsNotNullOrEmpty(result.Message);

                _compileVersionCommand.AssertWasNotCalled(c => c.Execute(Arg<object>.Is.Anything));
            }
        }
    }
}
