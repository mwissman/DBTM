using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class SqlStatementTests
    {
        [Test]
        public void CoalesceReturnsExistingSqlStatementIfNotNull()
        {
            SqlStatement statement = new SqlStatement("asdf","asdf","asdf");

            var actual = SqlStatement.Coalesce(statement);

            Assert.AreSame(statement, actual);
        }

        [Test]
        public void CoalesceReturnsNullSqlStatementIfNull()
        {
            var actual = SqlStatement.Coalesce(null);

            Assert.IsTrue(actual is EmptySqlStatement);
        }

        [Test]
        public void SettingIsEditableDoesNotChangeTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("","","");
            statement.IsEditable = false;
            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);
            
            statement.IsEditable = true;
            
            Assert.IsTrue(statement.IsSaved);
        }

        [Test]
        public void NewStatementHasId()
        {
            var statement = new SqlStatement("a", "", "");

            Assert.AreNotEqual(Guid.Empty, statement.Id);
        }

        [Test]
        public void SettingCanMoveUpDoesNotChangeTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            statement.CanMoveUp = false;
            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);

            statement.CanMoveUp = true;

            Assert.IsTrue(statement.IsSaved);
        }

        [Test]
        public void SettingCanMoveDownDoesNotChangeTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            statement.CanMoveDown = false;
            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);

            statement.CanMoveDown = true;

            Assert.IsTrue(statement.IsSaved);
        }

        [Test]
        public void SettingUpgradeSqlChangesTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            
            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);

            statement.UpgradeSQL = "some text";

            Assert.IsFalse(statement.IsSaved);
        }

        [Test]
        public void SettingRollbackSqlChangesTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("", "", "");

            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);

            statement.RollbackSQL = "some text";

            Assert.IsFalse(statement.IsSaved);
        }

        [Test]
        public void SettingDescriptonChangesTheSaveStatus()
        {
            SqlStatement statement = new SqlStatement("", "", "");

            statement.MarkAsSaved();

            Assert.IsTrue(statement.IsSaved);

            statement.Description = "some text";

            Assert.IsFalse(statement.IsSaved);
        }


        [Test]
        public void PropertyChangedIsFiredforIsSaved()
        {
            SqlStatement statement = new SqlStatement("","","");
            statement.MarkAsSaved();

            int propertyChangedCallCount = 0;
            List<string> propertiesChanged = new List<string>();

            statement.PropertyChanged += (o, args) =>
                                             {
                                                 propertyChangedCallCount++;
                                                 propertiesChanged.Add(args.PropertyName);
                                             };

            statement.Description = "asldjfasldjf";

            Assert.AreEqual(2, propertyChangedCallCount);
            CollectionAssert.Contains(propertiesChanged, ((Expression<Func<SqlStatement, object>>) (x => x.Description)).GetMemberName());
            CollectionAssert.Contains(propertiesChanged, ((Expression<Func<SqlStatement, object>>) (x => x.IsSaved)).GetMemberName());

        }

        
    }
}