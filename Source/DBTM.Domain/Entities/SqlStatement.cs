using System;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DBTM.Domain.Entities
{
    public class SqlStatement : INotifyPropertyChanged, IEntitySavedStateMonitor
    {
        private string _description;
        private string _upgradeSql;
        private string _rollbackSql;
        private bool _isSaved = true;
        private bool _canMoveUp;
        private bool _canMoveDown;
        private bool _isEditable;
        private Guid _id;

        [Obsolete("This Exists for Serialization only", true)]
        public SqlStatement()
        {
        }

        public SqlStatement(string description, string upgradeSql, string rollbackSql)
        {
            Description = description;
            UpgradeSQL = upgradeSql;
            RollbackSQL = rollbackSql;

            _isSaved = false;
            Id = Guid.NewGuid();
        }

        [XmlAttribute]
        public virtual Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    FirePropertyChangedEvent(x => x.Description, true);
                }
            }
        }

        [XmlElement("Description")]
        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    FirePropertyChangedEvent(x => x.Description, true);
                }
            }
        }

        [XmlElement("UpgradeSQL")]
        public virtual string UpgradeSQL
        {
            get
            {
                return _upgradeSql;
            }
            set
            {
                if (_upgradeSql != value)
                {
                    _upgradeSql = value;
                    FirePropertyChangedEvent(x => x.UpgradeSQL, true);
                }
            }
        }

        [XmlElement("RollbackSQL")]
        public virtual string RollbackSQL
        {
            get
            {
                return _rollbackSql;
            }
            set
            {
                if (_rollbackSql != value)
                {
                    _rollbackSql = value;
                    FirePropertyChangedEvent(x=>x.RollbackSQL, true);
                }
            }
        }

        [XmlIgnore]
        public virtual bool CanMoveUp
        {
            get { return _canMoveUp; }
            set
            {
                if (_canMoveUp != value)
                {
                    _canMoveUp = value;
                    FirePropertyChangedEvent(x => x.CanMoveUp, false);
                    FirePropertyChangedEvent(x => x.CanMoveUpAndIsEditable, false);
                }
            }
        }

        [XmlIgnore]
        public virtual bool CanMoveDown
        {
            get { return _canMoveDown; }
            set
            {
                if (_canMoveDown != value)
                {
                    _canMoveDown = value;
                    FirePropertyChangedEvent(x => x.CanMoveDown,false);
                    FirePropertyChangedEvent(x => x.CanMoveDownAndIsEditable, false);
                }
            }
        }

        [XmlIgnore]
        public virtual bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                if (_isEditable != value)
                {
                    _isEditable = value;
                    FirePropertyChangedEvent(x => x.IsEditable, false);
                    FirePropertyChangedEvent(x => x.CanMoveUpAndIsEditable, false);
                    FirePropertyChangedEvent(x => x.CanMoveDownAndIsEditable, false);
                }

            }
        }

        [XmlIgnore]
        public virtual bool CanMoveUpAndIsEditable
        {
            get { return CanMoveUp && IsEditable; }
        }

        [XmlIgnore]
        public virtual bool CanMoveDownAndIsEditable
        {
            get { return CanMoveDown && IsEditable; }
        }

        [XmlIgnore]
        public virtual bool IsSaved
        {
            get { return _isSaved; }
            private set
            {
                if (_isSaved != value)
                {
                    _isSaved = value;
                    FirePropertyChangedEvent(x => IsSaved, false);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static SqlStatement Coalesce(SqlStatement sqlStatement)
        {
            return sqlStatement ?? new EmptySqlStatement();
        }

        public virtual void MarkAsSaved()
        {
            IsSaved = true;
        }

        private void FirePropertyChangedEvent(Expression<Func<SqlStatement, object>> propertyNameFunc, bool makeUnsaved)
        {
            if (makeUnsaved)
            {
                IsSaved = false;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyNameFunc.GetMemberName()));
            }
        }
    }
}