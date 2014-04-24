using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseVersionTests
{
    [TestFixture]
    public partial class DatabaseVersionTests
    {

         [TestFixture]
        public class WhenCoalesceing
        {
            [Test]
            public void CoalesceReturnsExistingDatabaseVersionIfNotNull()
            {
                DatabaseVersion databaseVersion = new DatabaseVersion(123, DateTime.Now);

                var actual = DatabaseVersion.Coalesce(databaseVersion);

                Assert.AreSame(databaseVersion, actual);
            }

            [Test]
            public void CoalesceReturnsNullDatabaseVersionIfNull()
            {
                var actual = DatabaseVersion.Coalesce(null);

                Assert.IsTrue(actual is EmptyDatabaseVersion);
            }
        }
        
    }
}