using System;
using System.Linq;
using DBTM.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using DBTM.Domain.Entities;

namespace Tests.Domain
{
    [TestFixture]
    public class DatabaseRepositoryTests
    {
        private IXMLSerializer _serializer;
        private IDatabaseRepository _databaseRepository;
        private string _path;

        [SetUp]
        public void Setup()
        {
            _serializer = MockRepository.GenerateMock<IXMLSerializer>();
            _databaseRepository = new DatabaseRepository(_serializer);

            _path = @"c:\path\to\xml\database.xml";

        }

        [TearDown]
        public void TearDown()
        {
            _serializer.VerifyAllExpectations();
        }

        [Test]
        public void LoadDatabase()
        {


            Database deserializedDatabase = MockRepository.GeneratePartialMock<Database>("name");
            deserializedDatabase.Versions = new DatabaseVersionCollection()
                                                {
                                                    new DatabaseVersion(1, DateTime.Now)
                                                        {
                                                            PreDeploymentStatements =
                                                                new SqlStatementCollection()
                                                                    {
                                                                        new SqlStatement("1", "", "")
                                                                    },
                                                            PostDeploymentStatements =
                                                                new SqlStatementCollection()
                                                                    {
                                                                        new SqlStatement("1", "", "")
                                                                    }
                                                        },
                                                    new DatabaseVersion(2, DateTime.Now)
                                                        {
                                                            PreDeploymentStatements =
                                                                new SqlStatementCollection()
                                                                    {
                                                                        new SqlStatement("1", "", ""),
                                                                        new SqlStatement("2", "", ""),
                                                                        new SqlStatement("3", "", "")
                                                                    },
                                                            PostDeploymentStatements =
                                                                new SqlStatementCollection()
                                                                    {
                                                                        new SqlStatement("1", "", ""),
                                                                        new SqlStatement("2", "", ""),
                                                                        new SqlStatement("3", "", "")
                                                                    }
                                                        },
                                                };

            deserializedDatabase.Expect(d => d.MarkAsSaved());
            _serializer.Expect(s => s.Deserialize<Database>(_path)).Return(deserializedDatabase);

            var actualDatabase = _databaseRepository.Load(_path);
            
            Assert.AreSame(deserializedDatabase, actualDatabase);

            DatabaseVersion firstDatabaseVersion = actualDatabase.Versions.First();
            DatabaseVersion lastDatabaseVersion = actualDatabase.Versions.Last();
            Assert.IsFalse(firstDatabaseVersion.IsEditable);
            Assert.IsTrue(lastDatabaseVersion.IsEditable);

            VerifyPreDeploymentStatements(firstDatabaseVersion, lastDatabaseVersion);
            VerifyPostDeployementScripts(firstDatabaseVersion, lastDatabaseVersion);

            deserializedDatabase.VerifyAllExpectations();
        }

        private void VerifyPreDeploymentStatements(DatabaseVersion firstDatabaseVersion, DatabaseVersion lastDatabaseVersion)
        {
            SqlStatementCollection firstDatabaseVersionStatements = firstDatabaseVersion.PreDeploymentStatements;
            SqlStatementCollection lastDatabaseVersionStatements = lastDatabaseVersion.PreDeploymentStatements;

            Assert.AreEqual(1, firstDatabaseVersionStatements.Count);
            Assert.IsFalse(firstDatabaseVersionStatements.First().CanMoveDown);
            Assert.IsFalse(firstDatabaseVersionStatements.First().CanMoveUp);
            Assert.IsFalse(firstDatabaseVersionStatements.First().IsEditable);

            Assert.AreEqual(3, lastDatabaseVersionStatements.Count);
            Assert.IsTrue(lastDatabaseVersionStatements.First().CanMoveDown);
            Assert.IsFalse(lastDatabaseVersionStatements.First().CanMoveUp);
            Assert.IsTrue(lastDatabaseVersionStatements.First().IsEditable);

            Assert.IsFalse(lastDatabaseVersionStatements.Last().CanMoveDown);
            Assert.IsTrue(lastDatabaseVersionStatements.Last().CanMoveUp);
            Assert.IsTrue(lastDatabaseVersionStatements.Last().IsEditable);

            Assert.IsTrue(lastDatabaseVersionStatements.Where(s => s.Description == "2").Single().CanMoveDown);
            Assert.IsTrue(lastDatabaseVersionStatements.Where(s => s.Description == "2").Single().CanMoveUp);
        }

        private void VerifyPostDeployementScripts(DatabaseVersion firstDatabaseVersion, DatabaseVersion lastDatabaseVersion)
        {
            SqlStatementCollection firstDatabaseVersionStatements = firstDatabaseVersion.PostDeploymentStatements;
            SqlStatementCollection lastDatabaseVersionStatements = lastDatabaseVersion.PostDeploymentStatements;

            Assert.AreEqual(1, firstDatabaseVersionStatements.Count);
            Assert.IsFalse(firstDatabaseVersionStatements.First().CanMoveDown);
            Assert.IsFalse(firstDatabaseVersionStatements.First().CanMoveUp);
            Assert.IsFalse(firstDatabaseVersionStatements.First().IsEditable);

            Assert.AreEqual(3, lastDatabaseVersionStatements.Count);
            Assert.IsTrue(lastDatabaseVersionStatements.First().CanMoveDown);
            Assert.IsFalse(lastDatabaseVersionStatements.First().CanMoveUp);
            Assert.IsTrue(lastDatabaseVersionStatements.First().IsEditable);

            Assert.IsFalse(lastDatabaseVersionStatements.Last().CanMoveDown);
            Assert.IsTrue(lastDatabaseVersionStatements.Last().CanMoveUp);
            Assert.IsTrue(lastDatabaseVersionStatements.Last().IsEditable);

            Assert.IsTrue(lastDatabaseVersionStatements.Where(s => s.Description == "2").Single().CanMoveDown);
            Assert.IsTrue(lastDatabaseVersionStatements.Where(s => s.Description == "2").Single().CanMoveUp);
        }

        [Test]
        public void SaveDatabase()
        {
            Database database = MockRepository.GeneratePartialMock<Database>("name");

            database.Expect(d => d.MarkAsSaved());
            _serializer.Expect(s => s.Serialize(_path, database));
            
            _databaseRepository.Save(database, _path);

            database.VerifyAllExpectations();
        }

    }
}