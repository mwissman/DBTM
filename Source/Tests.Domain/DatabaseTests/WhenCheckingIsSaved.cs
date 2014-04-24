using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseTests
{
    [TestFixture]
    public partial class DatabaseTests
    {
        [TestFixture]
        public class WhenCheckingIsSaved
        {
            [Test]
            public void IfVersionsBecomesUnsavedSoDoesDatabase()
            {
                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);
                version.MarkAsSaved();

                Database database = new Database("blah");
                database.Versions.Add(version);
                database.MarkAsSaved();

                Assert.IsTrue(database.IsSaved);

                version.Description = "asdfasf";

                Assert.IsFalse(database.IsSaved);
            }
        }
    }
}