using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace DBTM.Domain.Entities
{
    public class DatabaseVersionCollection : IDatabaseVersionCollection
    {
        private readonly IList<DatabaseVersion> _internalCollection = new List<DatabaseVersion>();
        private bool _isSaved = true;

        public IEnumerator<DatabaseVersion> GetEnumerator()
        {
            return _internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(DatabaseVersion item)
        {
            item.PropertyChanged += DatabaseVersionPropertyChanged;
            _internalCollection.Add(item);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _internalCollection.ForEach(i => i.PropertyChanged -= DatabaseVersionPropertyChanged);
            _internalCollection.Clear();
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Remove(DatabaseVersion item)
        {
            var index = _internalCollection.IndexOf(item);
            var removed = _internalCollection.Remove(item);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            item.PropertyChanged -= DatabaseVersionPropertyChanged;
            return removed;
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
            set
            {
                if (_isSaved!=value)
                {
                    _isSaved = value;
                    FirePropertyChangedEvent(x=>x.IsSaved);
                }
            }
        }

        public int IndexOf(DatabaseVersion item)
        {
            return _internalCollection.IndexOf(item);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void MarkAsSaved()
        {
            IsSaved = true;
            _internalCollection.ForEach(v=>v.MarkAsSaved());
        }

        private void FireCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            IsSaved = false;

            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        private void FirePropertyChangedEvent(Expression<Func<DatabaseVersionCollection, object>> propertyNameFunc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyNameFunc.GetMemberName()));
            }
        }

        private void DatabaseVersionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetIsSavedPropertyName() && !((DatabaseVersion)sender).IsSaved)
            {
                IsSaved = false;
            }
        }

        private string GetIsSavedPropertyName()
        {
            return ((Expression<Func<DatabaseVersionCollection, object>>)(x => x.IsSaved)).GetMemberName();
        }
    }
}