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
    public class SqlStatementCollectionTests
    {
        private ISqlStatementCollection _collection;
        private SqlStatement _firstStatement;
        private SqlStatement _middleStatement;
        private SqlStatement _lastStatement;
        private bool _collectionChangedFired;

        [SetUp]
        public void Setup()
        {
            _firstStatement = new SqlStatement("description1", "upgrade1", "rollback1") { IsEditable = true};
            _middleStatement = new SqlStatement("description2", "upgrade2", "rollback2") { IsEditable = true };
            _lastStatement = new SqlStatement("description3", "upgrade3", "rollback3") { IsEditable = true };
           
            
            _collection = new SqlStatementCollection {_firstStatement, _middleStatement, _lastStatement};
            _collection.CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _collectionChangedFired = true;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void MoveItemUpMovesTheItemUpInTheCollection()
        {
            int position1 = 0;
            int position2 = 1;
            int position3 = 2;

            Assert.AreEqual(position1, _collection.IndexOf(_firstStatement));
            Assert.AreEqual(position2, _collection.IndexOf(_middleStatement));
            Assert.AreEqual(position3, _collection.IndexOf(_lastStatement));
            
            _collection.MoveItemUp(_lastStatement);

            Assert.IsTrue(_collectionChangedFired);
            Assert.AreEqual(position1, _collection.IndexOf(_firstStatement));
            Assert.AreEqual(position2, _collection.IndexOf(_lastStatement));
            Assert.AreEqual(position3, _collection.IndexOf(_middleStatement));


        }

        [Test]
        public void MoveItemDownMovesTheItemUpInTheCollection()
        {
            int position1 = 0;
            int position2 = 1;
            int position3 = 2;

            Assert.AreEqual(position1, _collection.IndexOf(_firstStatement));
            Assert.AreEqual(position2, _collection.IndexOf(_middleStatement));
            Assert.AreEqual(position3, _collection.IndexOf(_lastStatement));

            _collection.MoveItemDown(_firstStatement);

            Assert.IsTrue(_collectionChangedFired);
            Assert.AreEqual(position1, _collection.IndexOf(_middleStatement));
            Assert.AreEqual(position2, _collection.IndexOf(_firstStatement));
            Assert.AreEqual(position3, _collection.IndexOf(_lastStatement));
        }

        [Test]
        public void CanMoveUpReturnsFalseIfCollectionIsEmpty()
        {
            var collection = new SqlStatementCollection();
            Assert.IsFalse(collection.CanMoveUp(_lastStatement));
        }

        [Test]
        public void CanMoveDownReturnsFalseIfCollectionIsEmpty()
        {
            var collection = new SqlStatementCollection();
            Assert.IsFalse(collection.CanMoveDown(_firstStatement));
        }

        [Test]
        public void CanMoveUpReturnsFalseIfStatementIsFirstInTheListAndTrueIfStatementIsNotFirstInList()
        {
            Assert.IsFalse(_collection.CanMoveUp(_firstStatement));
            Assert.IsTrue(_collection.CanMoveUp(_middleStatement));
            Assert.IsTrue(_collection.CanMoveUp(_lastStatement));
        }

        [Test]
        public void CanMoveDownReturnsFalseIfStatementIsLastInTheListAndTrueIfStatementIsNotFirstInList()
        {
            Assert.IsTrue(_collection.CanMoveDown(_firstStatement));
            Assert.IsTrue(_collection.CanMoveDown(_middleStatement));
            Assert.IsFalse(_collection.CanMoveDown(_lastStatement));
        }

        [Test]
        public void CanMoveDownReturnsFalseIfOnly1ItemIsInTheCollection()
        {
            var collection = new SqlStatementCollection {_firstStatement};

            Assert.IsFalse(collection.CanMoveDown(_firstStatement));
        }

        [Test]
        public void CanMoveUpReturnsFalseIfOnly1ItemIsInTheCollection()
        {
            var collection = new SqlStatementCollection { _firstStatement };

            Assert.IsFalse(collection.CanMoveUp(_firstStatement));
        }

        [Test]
        public void CanMoveUpAndCanMoveDownReturnsFalseIfStatementIsEmpty()
        {
            Assert.IsFalse(_collection.CanMoveUp(new EmptySqlStatement()));
            Assert.IsFalse(_collection.CanMoveDown(new EmptySqlStatement()));
        }

        [Test]
        public void CanMoveUpChecksToSeeIfStatementIsEditable()
        {
            _firstStatement.IsEditable = false;
            _middleStatement.IsEditable = false;
            _lastStatement.IsEditable = false;

            Assert.IsFalse(_collection.CanMoveUp(_firstStatement));
            Assert.IsFalse(_collection.CanMoveUp(_middleStatement));
            Assert.IsFalse(_collection.CanMoveUp(_lastStatement));
        }

        [Test]
        public void CanMoveDownChecksToSeeIfStatementIsEditable()
        {
            _firstStatement.IsEditable = false;
            _middleStatement.IsEditable = false;
            _lastStatement.IsEditable = false;

            Assert.IsFalse(_collection.CanMoveDown(_firstStatement));
            Assert.IsFalse(_collection.CanMoveDown(_middleStatement));
            Assert.IsFalse(_collection.CanMoveDown(_lastStatement));
        }

        [Test]
        public void MarkAsSavedCascadesToChildren()
        {
            SqlStatementCollection collection = new SqlStatementCollection()
                                                    {
                                                        new SqlStatement("", "", ""),
                                                        new SqlStatement("", "", "")
                                                    };
            Assert.IsFalse(collection.IsSaved);
            collection.MarkAsSaved();

            Assert.IsTrue(collection.IsSaved);
            Assert.That(collection.All(x => x.IsSaved));
        }


        [Test]
        public void WhenAChildSqlStatementBecomesUnsavedSoDoesCollection()
        {
            SqlStatement statement = new SqlStatement("","","");
            SqlStatementCollection collection = new SqlStatementCollection()
                                                    {
                                                        statement
                                                    };
            collection.MarkAsSaved();

            Assert.IsTrue(collection.IsSaved);

            statement.Description = "asdlfjasdfl";
            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void WhenAChildIsMarkedAsSavedItDoesNotCascadeUp()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            SqlStatementCollection collection = new SqlStatementCollection()
                                                    {
                                                        statement
                                                    };
            collection.MarkAsSaved();

            Assert.IsTrue(collection.IsSaved);

            statement.Description = "asdlfjasdfl";
            Assert.IsFalse(collection.IsSaved);

            statement.MarkAsSaved();
            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void AddingAStatementMakesCollectionNotSaved()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            statement.MarkAsSaved();
            Assert.IsTrue(statement.IsSaved);

            SqlStatementCollection collection = new SqlStatementCollection();

            Assert.IsTrue(collection.IsSaved);

            collection.Add(statement);
            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void RemovingAStatementMakesCollectionNotSaved()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            statement.MarkAsSaved();
            Assert.IsTrue(statement.IsSaved);

            SqlStatementCollection collection = new SqlStatementCollection();
            collection.Add(statement);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.Remove(statement);
            
            Assert.IsFalse(collection.IsSaved);

            //collection unhooks from child
            statement.Description = "asdfasdf";
            Assert.IsFalse(statement.IsSaved);
        }

        [Test]
        public void MoveStatementUpAStatementMakesCollectionNotSaved()
        {
            SqlStatement statement1 = new SqlStatement("", "", "");
            SqlStatement statement2 = new SqlStatement("", "", "");
            statement1.MarkAsSaved();
            statement2.MarkAsSaved();
            Assert.IsTrue(statement1.IsSaved);
            Assert.IsTrue(statement2.IsSaved);

            SqlStatementCollection collection = new SqlStatementCollection();
            collection.Add(statement1);
            collection.Add(statement2);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.MoveItemUp(statement2);

            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void MoveStatementDownAStatementMakesCollectionNotSaved()
        {
            SqlStatement statement1 = new SqlStatement("", "", "");
            SqlStatement statement2 = new SqlStatement("", "", "");
            statement1.MarkAsSaved();
            statement2.MarkAsSaved();
            Assert.IsTrue(statement1.IsSaved);
            Assert.IsTrue(statement2.IsSaved);

            SqlStatementCollection collection = new SqlStatementCollection();
            collection.Add(statement1);
            collection.Add(statement2);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.MoveItemDown(statement1);

            Assert.IsFalse(collection.IsSaved);
        }

        [Test]
        public void ClearingAStatementMakesCollectionNotSaved()
        {
            SqlStatement statement = new SqlStatement("", "", "");
            statement.MarkAsSaved();
            Assert.IsTrue(statement.IsSaved);

            SqlStatementCollection collection = new SqlStatementCollection();
            collection.Add(statement);
            collection.MarkAsSaved();
            Assert.IsTrue(collection.IsSaved);

            collection.Clear();

            Assert.IsFalse(collection.IsSaved);

            //collection unhooks from child
            statement.Description = "asdfasdf";
            Assert.IsFalse(statement.IsSaved);
        }

        [Test]
        public void IsSavedFiresPropertyChanged()
        {
            SqlStatement statement = new SqlStatement("", "", "");

            statement.MarkAsSaved();
            Assert.IsTrue(statement.IsSaved);

            int propertyChangedCallCount = 0;
            List<string> propertiesChanged = new List<string>();

            SqlStatementCollection collection = new SqlStatementCollection();
            
            collection.PropertyChanged += (o, args) =>
            {
                propertyChangedCallCount++;
                propertiesChanged.Add(args.PropertyName);
            };

            collection.Add(statement);

            Assert.AreEqual(1, propertyChangedCallCount);
            CollectionAssert.Contains(propertiesChanged, ((Expression<Func<SqlStatement, object>>)(x => x.IsSaved)).GetMemberName());
        }


    }
}