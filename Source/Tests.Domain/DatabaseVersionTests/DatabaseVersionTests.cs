using System;
using NUnit.Framework;
using DBTM.Domain.Entities;

namespace Tests.Domain.DatabaseVersionTests
{
    [TestFixture]
    public partial class DatabaseVersionTests
    {        
        
        [TestFixture]
        public class WhenCheckingIfAVersionHasStatements
        {
            private DatabaseVersion _databaseVersion;

            [SetUp]
            public void Setup()
            {
                _databaseVersion = new DatabaseVersion(2, DateTime.Now);
            }

            [Test]
            public void HasStatementsIsFalseWhenNoStatementsExistInPrePostAndBackfill()
            {
                Assert.IsFalse(_databaseVersion.HasStatements);
            }

            [Test]
            public void HasStatemensIsTrueIfVersionHasAPreDeploymentStatement()
            {
                _databaseVersion.PreDeploymentStatements.Add(new SqlStatement("","",""));

                Assert.IsTrue(_databaseVersion.HasStatements);
            }

            [Test]
            public void HasStatemensIsTrueIfVersionHasABackfillStatement()
            {
                _databaseVersion.BackfillStatements.Add(new SqlStatement("", "", ""));

                Assert.IsTrue(_databaseVersion.HasStatements);
            }

            [Test]
            public void HasStatemensIsTrueIfVersionHasAPostDeploymentStatement()
            {
                _databaseVersion.PostDeploymentStatements.Add(new SqlStatement("", "", ""));

                Assert.IsTrue(_databaseVersion.HasStatements);
            }

        }

       
    }
}