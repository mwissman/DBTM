using System;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd.CreateVersionApplicationRunnerTests
{
    public partial class CreateVersionApplicationRunnerTests
    {
        [TestFixture]
        public class WhenRunning
        {
            private CreateVersionApplicationRunner _runner;
            private ICreateVersionArguments _arguments;
            private IDatabaseRepository _repository;
            private Database _database;
            private IMigrator _migrator;

            [SetUp]
            public void Setup()
            {
                _repository = MockRepository.GenerateMock<IDatabaseRepository>();
                _database = MockRepository.GenerateMock<Database>((string)null);
                _arguments = MockRepository.GenerateMock<ICreateVersionArguments>();
                _migrator = MockRepository.GenerateMock<IMigrator>();

                _runner = new CreateVersionApplicationRunner(_repository, _migrator);
            }

            [TearDown]
            public void TearDown()
            {
                _repository.VerifyAllExpectations();
                _migrator.VerifyAllExpectations();
            }

            [Test]
            public void LoadsDatabaseCallsCreateVersionOnHighestVersionAndSaves()
            {
                string dbschemaFilePath = "c:\\somewhere\\a.dbschema";

                _arguments.Stub(a => a.DatabaseSchemaFilePath).Return(dbschemaFilePath);

                _repository.Expect(r => r.Load(dbschemaFilePath)).Return(_database);
                _database.Expect(d => d.AddChangeset());
                _migrator.Expect(m => m.Migrate(_database));
                _repository.Expect(r => r.Save(_database, dbschemaFilePath));

                var result = _runner.Run(_arguments);

                Assert.IsTrue(result.Succeeded);
            }
        }
    }
}
