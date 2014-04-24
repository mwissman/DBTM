using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DBTM.Domain.Entities
{
    public interface ISqlStatementCollection : IEnumerable<SqlStatement>, INotifyCollectionChanged, IEntitySavedStateMonitor, INotifyPropertyChanged
    {
        void Add(SqlStatement item);
        void Clear();
        bool Remove(SqlStatement item);
        int Count{ get;}
        int IndexOf(SqlStatement item);
        bool CanMoveUp(SqlStatement item);
        bool CanMoveDown(SqlStatement item);
        void MoveItemUp(SqlStatement item);
        void MoveItemDown(SqlStatement item);
        void SetCanMoveUpDownOnAllStatements();
    }
}