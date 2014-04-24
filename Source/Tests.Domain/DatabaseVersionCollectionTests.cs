using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class DatabaseVersionCollectionTests
    {

        [Test]
        public void PropertyChangedIsFiredforIsSaved()
        {
            DatabaseVersionCollection versions = new DatabaseVersionCollection();
            versions.MarkAsSaved();

            int propertyChangedCallCount = 0;
            List<string> propertiesChanged = new List<string>();

            versions.PropertyChanged += (o, args) =>
                                            {
                                                propertyChangedCallCount++;
                                                propertiesChanged.Add(args.PropertyName);
                                            };

            versions.Add(new DatabaseVersion(1, DateTime.Now));

            Assert.AreEqual(1, propertyChangedCallCount);
            CollectionAssert.Contains(propertiesChanged, ((Expression<Func<DatabaseVersionCollection, object>>)(x => x.IsSaved)).GetMemberName());

        }

        [Test]
        public void AddingAStatementMakesCollectionNotSaved()
        {
            DatabaseVersion version = new DatabaseVersion(1,DateTime.Now);
            version.MarkAsSaved();
            Assert.IsTrue(version.IsSaved);

            DatabaseVersionCollection collection = new DatabaseVersionCollection();

            Assert.IsTrue(collection.IsSaved);

            collection.Add(version);
            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void RemovingAStatementMakesCollectionNotSaved()
        {
            DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);
            version.MarkAsSaved();
            Assert.IsTrue(version.IsSaved);

            DatabaseVersionCollection collection = new DatabaseVersionCollection();
            collection.Add(version);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.Remove(version);

            Assert.IsFalse(collection.IsSaved);

            //collection unhooks from child
            version.Description = "asdfasdf";
            Assert.IsFalse(version.IsSaved);
        }

        [Test]
        public void ClearingAStatementMakesCollectionNotSaved()
        {
            DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);
            version.MarkAsSaved();
            Assert.IsTrue(version.IsSaved);

            DatabaseVersionCollection collection = new DatabaseVersionCollection();
            collection.Add(version);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.Clear();

            Assert.IsFalse(collection.IsSaved);

            //collection unhooks from child
            version.Description = "asdfasdf";
            Assert.IsFalse(version.IsSaved);
        }

        [Test]
        public void IfAChildBecomesUnsavedItCascadesUp()
        {
            DatabaseVersion version = new DatabaseVersion(1, DateTime.Now);
            version.MarkAsSaved();

            DatabaseVersionCollection versions = new DatabaseVersionCollection { version };

            versions.MarkAsSaved();

            version.Description = "asdlfjsadf";

            Assert.IsFalse(version.IsSaved);
            Assert.IsFalse(versions.IsSaved);
        }

        [Test]
        public void MarkAsSaveCascadesDown()
        {
            DatabaseVersion version1 = new DatabaseVersion(1, DateTime.Now);
            DatabaseVersion version2 = new DatabaseVersion(1, DateTime.Now);
            DatabaseVersionCollection versions = new DatabaseVersionCollection { version1,version2 };

            version1.Description = "asdlfjsadf";
            version2.Description = "asdlfjsadf";

            versions.MarkAsSaved();

            Assert.IsTrue(versions.IsSaved);
            Assert.That(versions.All(v=>v.IsSaved));

        }

    }
}