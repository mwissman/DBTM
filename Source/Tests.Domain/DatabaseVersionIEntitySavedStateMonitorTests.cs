using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class DatabaseVersionIEntitySavedStateMonitorTests
    {
        [Test]
        public void NewDatabaseVersionIsNotSavedWhenFirstCreated()
        {
            var databaseVersion = new DatabaseVersion(1,DateTime.Now);

            Assert.IsFalse(databaseVersion.IsSaved);
        }

        [Test]
        public void DatabaseVersionIsSavedAfterSomeoneCallsSave()
        {
            var databaseVersion = new DatabaseVersion(1, DateTime.Now);

            databaseVersion.MarkAsSaved();

            Assert.IsTrue(databaseVersion.IsSaved);
        }

        [Test]
        public void IsSavedWhenLoadedThroughSerialization()
        {
            var databaseVersion = (DatabaseVersion)Activator.CreateInstance(typeof(DatabaseVersion));

            Assert.IsTrue(databaseVersion.IsSaved);
        }

        [Test]
        public void AddingASqlStatementToASavedDatabaseVersionMakesItNotSaved()
        {
            var databaseVersion = new DatabaseVersion(1, DateTime.Now);

            databaseVersion.MarkAsSaved();
            databaseVersion.PreDeploymentStatements.Add(new SqlStatement("","",""));

            Assert.IsFalse(databaseVersion.IsSaved);
        }
    }
}