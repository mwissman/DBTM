using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseTests
{
    [TestFixture]
    public partial class DatabaseTests
    {
        [TestFixture]
        public class WhenIndexingADatabaseObject
        {
            [Test]
            public void IndexReturnsDatabaseVersionByVersionId()
            {
                int firstVersion = 1;
                int secondVersion = 2;

                var version1 = new DatabaseVersion(firstVersion, DateTime.Now);
                var version2 = new DatabaseVersion(secondVersion, DateTime.Now);

                var databaseVersions = new DatabaseVersionCollection { version1, version2 };

                Database database = new Database("name") { Versions = databaseVersions };

                Assert.AreSame(version1, database[firstVersion]);
                Assert.AreSame(version2, database[secondVersion]);
            }

            [Test]
            [ExpectedException(typeof(IndexOutOfRangeException))]
            public void IndexThrowsExceptionIfDatabaseDoesNotContainThatVersionNumber()
            {
                int firstVersion = 1;

                Database database = new Database("name");

                var versionIdDoesNotExist = database[firstVersion];
            }
        }
    }
}