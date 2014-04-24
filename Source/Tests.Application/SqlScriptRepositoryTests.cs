using System;
using System.Reflection;
using DBTM.Application;
using DBTM.Domain;
using DBTM.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application
{
    [TestFixture]
    public class SqlScriptRepositoryTests
    {
        private IStreamReader _streamReader;
        private ISqlScriptRepository _sqlScriptRepository;

        [SetUp]
        public void Setup()
        {
            _streamReader = MockRepository.GenerateMock<IStreamReader>();

            _sqlScriptRepository = new SqlScriptRepository(_streamReader);

        }

        [TearDown]
        public void TearDown()
        {
            _streamReader.VerifyAllExpectations();
        }

        [Test]
        public void LoadDisconnectUserReadsFromFile_ReplacesDatabaseName()
        {
            string disconnectTemplate = "aslkdjf alskdjf [DATABASE_NAME] 123048 SDAF";
            string databaseName = "databaseName";

            _streamReader.Expect(sr => sr.ReadEmbeddedFile(Arg<Assembly>.Matches(a=>a.GetName().Name=="DBTM.Application"),Arg<string>.Is.Equal(".SqlScripts.Disconnect.sql"))).Return(
                disconnectTemplate);

            var actual = _sqlScriptRepository.LoadDisconnectUser(databaseName);

            Assert.AreEqual(disconnectTemplate.Replace("[DATABASE_NAME]", databaseName), actual);


        }

        [Test]
        public void LoadCreateSchemaReadsFromFile_ReplacesDatabaseNameAndDbFilesDirector()
        {
            string disconnectTemplate = "aslkdjf alskdjf [DATABASE_NAME] 123048 SDAF [DATABASE_FILE_DIRECTORY][DATABASE_NAME].mdf";
            string databaseName = "databaseName";
            string directory = "C:\\foo\\bar";

            _streamReader.Expect(sr => sr.ReadEmbeddedFile(Arg<Assembly>.Matches(a=>a.GetName().Name=="DBTM.Application"),Arg<string>.Is.Equal(".SqlScripts.CreateSchema.sql" ))).Return(disconnectTemplate);
            
            var actual = _sqlScriptRepository.LoadCreateSchema(databaseName, directory);

            Assert.AreEqual(disconnectTemplate.Replace("[DATABASE_NAME]", databaseName).Replace("[DATABASE_FILE_DIRECTORY]", directory), actual);

        }

        [Test]
        public void LoadDropSchemaReadsFromFile_ReplacesDatabaseNameAndDbFilesDirector()
        {
            string dropSchemaTemplate =
                "aslkdjf alskdjf [DATABASE_NAME] 123048 SDAF ";
            string databaseName = "databaseName";

            _streamReader.Expect(sr => sr.ReadEmbeddedFile(Arg<Assembly>.Matches(a=>a.GetName().Name=="DBTM.Application"),Arg<string>.Is.Equal(".SqlScripts.DropSchema.sql"))).Return(dropSchemaTemplate);

            var actual = _sqlScriptRepository.LoadDropSchema(databaseName);

            Assert.AreEqual(
                dropSchemaTemplate.Replace("[DATABASE_NAME]", databaseName), actual);

        }

        [Test]
        public void LoadDBTMUpgradeAndRollbackScripts()
        {
            string upgrade = "--upgrade";
            string rollback = "--rollback";

            _streamReader.Expect(sr => sr.ReadEmbeddedFile(Arg<Assembly>.Matches(a => a.GetName().Name == "DBTM.Application"), Arg<string>.Is.Equal(".SqlScripts.DBTMHistoryRollback.sql"))).Return(rollback);
            _streamReader.Expect(sr => sr.ReadEmbeddedFile(Arg<Assembly>.Matches(a => a.GetName().Name == "DBTM.Application"), Arg<string>.Is.Equal(".SqlScripts.DBTMHistoryUpgrade.sql"))).Return(upgrade);

            var actual = _sqlScriptRepository.LoadHistroySql();

            Assert.AreEqual(upgrade,actual.Upgrade.ToString());
            Assert.AreEqual(rollback, actual.Rollback.ToString());

            Assert.IsInstanceOf<GoSeperatedCompiledSql>(actual);

        }
    }
}