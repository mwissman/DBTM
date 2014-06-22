using System;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain
{
    [TestFixture]
    public class MigratorTests
    {
        private IGuidFactory _guidFactory;
        private Migrator _migrator;

        [SetUp]
        public void Setup()
        {
            _guidFactory = MockRepository.GenerateMock<IGuidFactory>();

            _migrator = new Migrator(_guidFactory);


        }

        [TearDown]
        public void TearDown()
        {
            _guidFactory.VerifyAllExpectations();
        }

        [Test]
        public void EnsuresAllStatementsHaveIds()
        {
            Guid otherGuid = Guid.NewGuid();
            Guid guid = Guid.NewGuid();

            _guidFactory.Stub(f => f.Create()).Return(guid);
            
            var database = new Database("test");
            var version1 = database.AddChangeset();
            var sqlStatement1 = new SqlStatement("a", "b", "c"){Id = Guid.Empty};
            var sqlStatement2 = new SqlStatement("a", "b", "c") { Id = Guid.Empty };
          
            var sqlStatement5 = new SqlStatement("a", "b", "c") { Id = Guid.Empty };
            var sqlStatement6 = new SqlStatement("a", "b", "c") { Id = Guid.Empty };
            var sqlStatement7 = new SqlStatement("a", "b", "c") { Id = Guid.Empty };
            var sqlStatement8 = new SqlStatement("a", "b", "c"){Id = otherGuid};

            version1.PreDeploymentStatements.Add(sqlStatement1);
            version1.PreDeploymentStatements.Add(sqlStatement2);

            version1.PostDeploymentStatements.Add(sqlStatement5);
            version1.PostDeploymentStatements.Add(sqlStatement6);

            var version2 = database.AddChangeset();
            version2.PreDeploymentStatements.Add(sqlStatement7);
            version2.PreDeploymentStatements.Add(sqlStatement8);

            _migrator.EnsureStatementsHaveIds(database);


            Assert.AreEqual(sqlStatement1.Id,guid);
            Assert.AreEqual(sqlStatement2.Id,guid);
            Assert.AreEqual(sqlStatement5.Id,guid);
            Assert.AreEqual(sqlStatement6.Id,guid);
            Assert.AreEqual(sqlStatement7.Id,guid);
            Assert.AreEqual(sqlStatement8.Id,otherGuid);

        }

    }
}