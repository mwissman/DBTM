using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class DatabaseIEntitySavedStateMonitorTests
    {
        [Test]
        public void NewDatabaseIsNotSavedWhenFirstCreated()
        {
            Database database = new Database("blah");

            Assert.IsFalse(database.IsSaved);
        }

        [Test]
        public void DatabaseIsSavedAfterSomeoneCallsSave()
        {
            Database database = new Database("blah");

            database.MarkAsSaved();

            Assert.IsTrue(database.IsSaved);
        }

        [Test]
        public void IsSavedWhenLoadedThroughSerialization()
        {
            var database = (Database)Activator.CreateInstance(typeof(Database));

            Assert.IsTrue(database.IsSaved);
        }

        [Test]
        public void AddingAChangedSetToASavedDatabaseMakesItNotSaved()
        {
            Database database = new Database("blah");

            database.MarkAsSaved();
            database.AddChangeset();

            Assert.IsFalse(database.IsSaved);
        }
    }
}