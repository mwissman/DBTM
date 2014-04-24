using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DBTM.Domain.Entities
{
    public interface IDatabaseVersionCollection : IEnumerable<DatabaseVersion>, INotifyCollectionChanged, INotifyPropertyChanged, IEntitySavedStateMonitor
    {
        void Add(DatabaseVersion item);
        void Clear();
        bool Remove(DatabaseVersion item);
        int Count { get; }
        int IndexOf(DatabaseVersion item);
    }
}