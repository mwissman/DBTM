using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseTests
{
    [TestFixture]
    public partial class DatabaseTests
    {
        [TestFixture]
        public class WhenMarkingDatabaseAsSaved
        {
            [Test]
            public void MarkAsSavedCascadesToVersions()
            {
                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);

                Database database = new Database("blah");
                database.Versions.Add(version);

                database.MarkAsSaved();

                Assert.IsTrue(database.IsSaved);
                Assert.IsTrue(database.Versions.IsSaved);

            }

        }
    }
}