using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace DBTM.Domain.Entities
{
    public class SqlStatementCollection : ISqlStatementCollection
    {
        private readonly IList<SqlStatement> _internalCollection = new List<SqlStatement>();
        private bool _isSaved = true;

        public virtual void Add(SqlStatement item)
        {
            item.PropertyChanged+=SqlStatementPropertyChanged;

            _internalCollection.Add(item);
            SetCanMoveUpDownOnAllStatements();
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public virtual void Clear()
        {
            _internalCollection.ForEach(i => i.PropertyChanged -= SqlStatementPropertyChanged);
            _internalCollection.Clear();
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public virtual bool Remove(SqlStatement item)
        {
            var index = _internalCollection.IndexOf(item);
            var removed = _internalCollection.Remove(item);
            SetCanMoveUpDownOnAllStatements();
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

            item.PropertyChanged -= SqlStatementPropertyChanged;
            return removed;
        }

        public virtual void MoveItemUp(SqlStatement item)
        {
            int oldIndex = _internalCollection.IndexOf(item);
            int newIndex = oldIndex - 1;
            SqlStatement affectedItem = _internalCollection[newIndex];

            _internalCollection[newIndex] = item;
            _internalCollection[oldIndex] = affectedItem;

            SetCanMoveUpDownOnAllStatements();

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(
                                      NotifyCollectionChangedAction.Move,
                                      item,
                                      newIndex,
                                      oldIndex));
        }


        public virtual void MoveItemDown(SqlStatement item)
        {
            int oldIndex = _internalCollection.IndexOf(item);
            int newIndex = oldIndex + 1;
            SqlStatement affectedItem = _internalCollection[newIndex];

            _internalCollection[newIndex] = item;
            _internalCollection[oldIndex] = affectedItem;

            SetCanMoveUpDownOnAllStatements();

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(
                                      NotifyCollectionChangedAction.Move,
                                      item,
                                      newIndex,
                                      oldIndex));
        }

        public virtual void SetCanMoveUpDownOnAllStatements()
        {
            foreach (var statement in _internalCollection)
            {
                statement.CanMoveDown = CanMoveDown(statement);
                statement.CanMoveUp = CanMoveUp(statement);
            }
        }

        [XmlIgnore]
        public int Count
        {
            get { return _internalCollection.Count; }
        }

        [XmlIgnore]
        public bool IsSaved
        {
            get { return _isSaved; }
            private set 
            {
                if (_isSaved != value)
                {
                    _isSaved = value;
                    FirePropertyChangedEvent(x => x.IsSaved);
                }
            }
        }

        public void MarkAsSaved()
        {
            IsSaved = true;
            _internalCollection.ForEach(s => s.MarkAsSaved());
        }

        public int IndexOf(SqlStatement item)
        {
            return _internalCollection.IndexOf(item);
        }

        public virtual bool CanMoveUp(SqlStatement item)
        {
            if (!item.IsEditable)
            {
                return false;
            }

            if (Count == 0 || item is EmptySqlStatement)
            {
                return false;
            }
            if (_internalCollection.IndexOf(item) == 0)
            {
                return false;
            }
            if (_internalCollection.IndexOf(item) == Count - 1)
            {
                return true;
            }
            return true;
        }

        public virtual bool CanMoveDown(SqlStatement item)
        {
            if (!item.IsEditable)
            {
                return false;
            }

            if (Count == 0 || item is EmptySqlStatement)
            {
                return false;
            }
            if (_internalCollection.IndexOf(item) == 0 && _internalCollection.IndexOf(item) != Count - 1)
            {
                return true;
            }
            if (_internalCollection.IndexOf(item) == Count - 1)
            {
                return false;
            }
            return true;
        }

        public IEnumerator<SqlStatement> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void FireCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            IsSaved = false;

            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        private void SqlStatementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetIsSavedPropertyName() && !((SqlStatement)sender).IsSaved)
            {
                IsSaved = false;
            }
        }

        private string GetIsSavedPropertyName()
        {
            return ((Expression<Func<SqlStatementCollection, object>>)(x => x.IsSaved)).GetMemberName();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(Expression<Func<SqlStatementCollection, object>> propertyNameFunc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyNameFunc.GetMemberName()));
            }
        }
    }
}