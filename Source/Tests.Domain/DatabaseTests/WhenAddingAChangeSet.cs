using System;
using System.Linq;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain.DatabaseTests
{
    [TestFixture]
    public partial class DatabaseTests
    {
        [TestFixture]
        public class WhenAddingAChangeSet
        {
            [Test]
            public void AddChangesetIncrimentsVersionNumberAndMakeHighestVersionEditable()
            {
                int firstVersion = 1;
                int expectedVersion = 2;
                var databaseVersion1 = MockRepository.GenerateMock<DatabaseVersion>(firstVersion, DateTime.Now);
                databaseVersion1.IsEditable = true;
                databaseVersion1.VersionNumber = 1;
                databaseVersion1.Stub(dv => dv.HasStatements).Return(true);

                Database database = new Database("name") {Versions = new DatabaseVersionCollection { databaseVersion1 } };
                var actual = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.AreEqual(expectedVersion, database.Versions.Max(v => v.VersionNumber));
                Assert.AreEqual(expectedVersion, actual.VersionNumber);
                Assert.AreEqual(0, actual.PreDeploymentStatements.Count);
                Assert.That(actual.Created, Is.InRange(DateTime.Now.AddSeconds(-1), DateTime.Now));
                Assert.IsTrue(actual.IsEditable);
                Assert.IsFalse(database.Versions.First().IsEditable);
                Assert.IsFalse(database.IsSaved);
            }

            [Test]
            public void AddChangesetIncrementsVersionNumberIfCurrentVersionIsEmpty()
            {
                int expectedUpdatedVersion = 3;

                var databaseVersion1 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion1.VersionNumber = 1;
                databaseVersion1.IsEditable = false;

                var databaseVersion2 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion2.VersionNumber = 2;
                databaseVersion2.IsEditable = true;
                databaseVersion2.Stub(dv => dv.HasStatements).Return(false);
                databaseVersion2.Created = DateTime.MinValue;
               
                Database database = new Database("name") {Versions = new DatabaseVersionCollection {databaseVersion1, databaseVersion2}};

                var actual = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.AreEqual(expectedUpdatedVersion, database.Versions.Max(v => v.VersionNumber));
                Assert.AreEqual(expectedUpdatedVersion, actual.VersionNumber);

                Assert.That(actual.Created, Is.InRange(DateTime.Now.AddSeconds(-1), DateTime.Now));
                Assert.IsTrue(actual.IsEditable);

                Assert.IsFalse(database.Versions.First().IsEditable);
                Assert.IsFalse(database.IsSaved);
            }

            [Test]
            public void AddingFirstChangeCreatesFirstDatabaseVersion()
            {
                int expectedVersion = 1;

                Database database = new Database("name");

                var actual = database.AddChangeset();

                Assert.AreEqual(1, database.Versions.Count);
                Assert.AreEqual(expectedVersion, database.Versions.Max(v => v.VersionNumber));
                Assert.AreEqual(expectedVersion, actual.VersionNumber);
                
                Assert.That(actual.Created, Is.InRange(DateTime.Now.AddSeconds(-1), DateTime.Now));
                Assert.IsTrue(actual.IsEditable);
                Assert.IsTrue(database.Versions.First().IsEditable);
            }

            [Test]
            public void AddingFirstChangeSetSetsFirstVersionToBaseLineVersion()
            {
                Database database = new Database("name");

                var actual = database.AddChangeset();

                Assert.AreEqual(1, database.Versions.Count);
                Assert.IsTrue(actual.IsBaseline);
            }

            [Test]
            public void AddingNotFirstChangeSetDoesNotSetBaselineFlag()
            {
                Database database = new Database("name");
           
                var firstDbVersion = database.AddChangeset();

                firstDbVersion.PreDeploymentStatements.Add(new SqlStatement("test","asdf","asdf"));

                var secondDbVersion = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.IsFalse(secondDbVersion.IsBaseline);
            }

            [Test]
            public void WhenFirstVersionIsNotTheBaselineAndPreviousVersionHasStatementsMarkNewVersionAsBaseline()
            {
                Database database = new Database("name");

                var firstDbVersion = database.AddChangeset();
                firstDbVersion.IsBaseline = false;
                firstDbVersion.PreDeploymentStatements.Add(new SqlStatement("test", "asdf", "asdf"));

                var secondDbVersion = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.IsTrue(secondDbVersion.IsBaseline);
            }

            [Test]
            public void WhenFirstVersionIsNotTheBaselineAndPreviousVersionDoesNotHaveStatementsMarkCurrentVersionIsBaseline()
            {
                int expectedUpdatedVersion = 3;

                var databaseVersion1 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion1.VersionNumber = 1;
                databaseVersion1.IsEditable = false;

                var databaseVersion2 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion2.VersionNumber = 2;
                databaseVersion2.IsEditable = true;
                databaseVersion2.Stub(dv => dv.HasStatements).Return(false);
                databaseVersion2.Created = DateTime.MinValue;

                Database database = new Database("name") { Versions = new DatabaseVersionCollection { databaseVersion1, databaseVersion2 } };

                var actual = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.AreEqual(expectedUpdatedVersion, database.Versions.Max(v => v.VersionNumber));
                Assert.AreEqual(expectedUpdatedVersion, actual.VersionNumber);

               Assert.IsTrue(actual.IsBaseline);
            }

            [Test]
            public void WhenFirstVersionIsNotTheBaselineAndPreviousVersionDoesNotHaveStatementsAndHistoryTrackingIsDisabledMarkCurrentVersionIsNotTheBaseline()
            {
                int expectedUpdatedVersion = 3;

                var databaseVersion1 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion1.VersionNumber = 1;
                databaseVersion1.IsEditable = false;

                var databaseVersion2 = MockRepository.GenerateStub<DatabaseVersion>(0, DateTime.Now);
                databaseVersion2.VersionNumber = 2;
                databaseVersion2.IsEditable = true;
                databaseVersion2.Stub(dv => dv.HasStatements).Return(false);
                databaseVersion2.Created = DateTime.MinValue;

                Database database = new Database("name") { Versions = new DatabaseVersionCollection { databaseVersion1, databaseVersion2 } };
                database.DisableHistoryTracking = true;

                var actual = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.AreEqual(expectedUpdatedVersion, database.Versions.Max(v => v.VersionNumber));
                Assert.AreEqual(expectedUpdatedVersion, actual.VersionNumber);

                Assert.IsFalse(actual.IsBaseline);
            }

            [Test]
            public void WhenHistoryTrackingIsTurnedOffAndDatabaseDoesNotHaveABaselineNewVersionAndPreviousVersionHasStatementsAddingAChangesetDoesNotCreateABaseLine()
            {
                Database database = new Database("name");
                database.DisableHistoryTracking = true;

                var firstDbVersion = database.AddChangeset();
                firstDbVersion.IsBaseline = false;
                firstDbVersion.PreDeploymentStatements.Add(new SqlStatement("test", "asdf", "asdf"));

                var secondDbVersion = database.AddChangeset();

                Assert.AreEqual(2, database.Versions.Count);
                Assert.IsFalse(secondDbVersion.IsBaseline);

            }

        }
    }
}