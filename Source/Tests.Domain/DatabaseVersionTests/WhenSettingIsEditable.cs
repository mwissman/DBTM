using System;
using System.Linq;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseVersionTests
{
    [TestFixture]
    public partial class DatabaseVersionTests
    {

        [TestFixture]
        public class WhenSettingIsEditable
        {
            [Test]
            public void SettingIsEditableCascadesValueToStatements()
            {
                var databaseVersion = new DatabaseVersion(2, DateTime.Now);
                databaseVersion.PreDeploymentStatements.Add(new SqlStatement("", "", ""));
                databaseVersion.PreDeploymentStatements.Add(new SqlStatement("", "", ""));

                Assert.IsFalse(databaseVersion.IsEditable);
                Assert.That(databaseVersion.PreDeploymentStatements.All(s => s.IsEditable == false));

                databaseVersion.IsEditable = true;

                Assert.IsTrue(databaseVersion.IsEditable);
                Assert.That(databaseVersion.PreDeploymentStatements.All(s => s.IsEditable == true));
            }

        }
    }
}