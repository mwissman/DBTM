using System;
using System.Linq.Expressions;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.DatabaseTests
{
    [TestFixture]
    public class BaselineTests
    {
        [Test]
        public void BaselineNumberIsIntMaxIfNoBaseline()
        {
            Database database = new Database("foo");

            Assert.AreEqual(int.MaxValue, database.BaseLineVersionNumber);
            Assert.IsFalse(database.HasBaseline);
        }

        [Test]
        public void BaselineNumberVersionNumberOfBaseline()
        {
            Database database = new Database("foo");

            database.AddChangeset();
   
            Assert.AreEqual(1, database.BaseLineVersionNumber);
            Assert.IsTrue(database.HasBaseline);
        }

        [Test]
        public void NotifyPropertyChangeIsTriggeredWhenBaseLineVersionNumberChanges()
        {
            var database = new Database("foo");


            var baseLineVersionNumberPropertyName=((Expression<Func<Database, object>>)(x => x.BaseLineVersionNumber)).GetMemberName();
            var hasBaselinePropertyName=((Expression<Func<Database, object>>)(x => x.HasBaseline)).GetMemberName();

            var baselineVersionNumberTriggeredPropertyChange = false;
            var hasBaselineTriggeredPropertyChange = false;

            database.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == baseLineVersionNumberPropertyName)
                    {
                        baselineVersionNumberTriggeredPropertyChange = true;
                    }

                    if (e.PropertyName == hasBaselinePropertyName)
                    {
                        hasBaselineTriggeredPropertyChange = true;
                    }
                };

            database.AddChangeset();

            Assert.IsTrue(baselineVersionNumberTriggeredPropertyChange);
            Assert.IsTrue(hasBaselineTriggeredPropertyChange);
        }

        [Test]
        public void ChangingDisableHistoryTrackingSetDatabaseToNotSaved()
        {
            Database database = new Database("foo");

            database.MarkAsSaved();

            database.DisableHistoryTracking = true;

            Assert.IsFalse(database.IsSaved);
        }
    }
}