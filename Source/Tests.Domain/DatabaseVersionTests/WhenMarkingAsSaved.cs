using System;
using System.Linq.Expressions;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Domain.DatabaseVersionTests
{
    [TestFixture]
    public partial class DatabaseVersionTests
    {
        [TestFixture]
        public class WhenMarkingAsSaved
        {
            [Test]
            public void IsSavedFiresPropertyChanged()
            {
                int propertyChangedCallCount = 0;
                List<string> propertiesChanged = new List<string>();

                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);
                version.MarkAsSaved();

                version.PropertyChanged += (o, args) =>
                {
                    propertyChangedCallCount++;
                    propertiesChanged.Add(args.PropertyName);
                };

                version.Description = "asdf";

                Assert.AreEqual(2, propertyChangedCallCount);
                CollectionAssert.Contains(propertiesChanged, ((Expression<Func<DatabaseVersion, object>>)(x => x.IsSaved)).GetMemberName());
                CollectionAssert.Contains(propertiesChanged, ((Expression<Func<DatabaseVersion, object>>)(x => x.Description)).GetMemberName());
            }

            [Test]
            public void IsSavedIsSetToFalseWhenBackfillStatementsBecomesNotSaved()
            {
                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now)
                {
                    BackfillStatements = new SqlStatementCollection(),
                };

                version.MarkAsSaved();
                Assert.IsTrue(version.IsSaved);
                version.BackfillStatements.Add(new SqlStatement("", "", ""));
                Assert.IsFalse(version.IsSaved);
            }

            [Test]
            public void IsSavedIsSetToFalseWhenPreDeploymentStatementsBecomesNotSaved()
            {
                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now)
                {
                    PreDeploymentStatements = new SqlStatementCollection(),
                };

                version.MarkAsSaved();
                Assert.IsTrue(version.IsSaved);
                version.PreDeploymentStatements.Add(new SqlStatement("", "", ""));
                Assert.IsFalse(version.IsSaved);
            }

            [Test]
            public void IsSavedIsSetToFalseWhenPostDeploymentStatementsBecomesNotSaved()
            {
                DatabaseVersion version = new DatabaseVersion(1, DateTime.Now)
                {
                    PostDeploymentStatements = new SqlStatementCollection(),
                };

                version.MarkAsSaved();
                Assert.IsTrue(version.IsSaved);
                version.PostDeploymentStatements.Add(new SqlStatement("", "", ""));
                Assert.IsFalse(version.IsSaved);
            }

            [Test]
            public void MarkAsSavedCascadesToAllCollections()
            {
                DatabaseVersion version = new DatabaseVersion(2, DateTime.Now);
                version.BackfillStatements.Add(new SqlStatement("", "", ""));
                version.PreDeploymentStatements.Add(new SqlStatement("", "", ""));
                version.PostDeploymentStatements.Add(new SqlStatement("", "", ""));

                Assert.IsFalse(version.IsSaved);
                Assert.IsFalse(version.BackfillStatements.IsSaved);
                Assert.IsFalse(version.PreDeploymentStatements.IsSaved);
                Assert.IsFalse(version.PostDeploymentStatements.IsSaved);

                version.MarkAsSaved();

                Assert.IsTrue(version.IsSaved);
                Assert.IsTrue(version.BackfillStatements.IsSaved);
                Assert.IsTrue(version.PreDeploymentStatements.IsSaved);
                Assert.IsTrue(version.PostDeploymentStatements.IsSaved);
            }
        }
    }
}