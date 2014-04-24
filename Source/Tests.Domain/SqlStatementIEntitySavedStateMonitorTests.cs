using System;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class SqlStatementIEntitySavedStateMonitorTests
    {
        [Test]
        public void SqlStatementIsNotSavedWhenFirstCreated()
        {
            var sqlStatement = new SqlStatement("","","");

            Assert.IsFalse(sqlStatement.IsSaved);
        }

        [Test]
        public void SqlStatementIsSavedAfterSomeoneCallsSave()
        {
            var sqlStatement = new SqlStatement("", "", "");

            sqlStatement.MarkAsSaved();

            Assert.IsTrue(sqlStatement.IsSaved);
        }

        [Test]
        public void IsSavedWhenLoadedThroughSerialization()
        {
            var sqlStatement = (SqlStatement)Activator.CreateInstance(typeof(SqlStatement));

            Assert.IsTrue(sqlStatement.IsSaved);
        }

        [Test]
        public void ChangingPropertiesMakesItDirty()
        {
            var sqlStatement = new SqlStatement("", "", "");

            sqlStatement.MarkAsSaved();
            sqlStatement.Description = "description";
            Assert.IsFalse(sqlStatement.IsSaved);

            sqlStatement.MarkAsSaved();
            sqlStatement.RollbackSQL = "rollback";
            Assert.IsFalse(sqlStatement.IsSaved);

            sqlStatement.MarkAsSaved();
            sqlStatement.UpgradeSQL = "upgrade";
            Assert.IsFalse(sqlStatement.IsSaved);

        }

        [Test]
        public void SettingPropertiesToSameValueDoesNotMakeUnsaved()
        {
            string description = "description";
            string rollback = "rollback";
            string upgrade = "upgrade";

            var sqlStatement = new SqlStatement(description, upgrade, rollback);
            sqlStatement.MarkAsSaved();

            sqlStatement.Description = description;
            Assert.IsTrue(sqlStatement.IsSaved);

            sqlStatement.RollbackSQL = rollback;
            Assert.IsTrue(sqlStatement.IsSaved);

            sqlStatement.UpgradeSQL = upgrade;
            Assert.IsTrue(sqlStatement.IsSaved);
        }


    }
}