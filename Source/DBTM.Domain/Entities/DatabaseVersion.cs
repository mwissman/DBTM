using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.Text;

namespace DBTM.Domain.Entities
{
    public class DatabaseVersion : IEntitySavedStateMonitor, INotifyPropertyChanged
    {
        private IDictionary<SqlStatementType, SqlStatementCollection> _statements =
            new Dictionary<SqlStatementType, SqlStatementCollection>()
                {
                    {SqlStatementType.PreDeployment, new SqlStatementCollection()},
                    {SqlStatementType.Backfill, new SqlStatementCollection()},
                    {SqlStatementType.PostDeployment, new SqlStatementCollection()},

                };

        [XmlIgnore]
        private bool _isSaved = true;

        private int _versionNumber;
        private DateTime _created;
        private string _cardNumber;
        private string _description;
        private bool _isEditable;
        private bool _isBaseline;

        [Obsolete("This Exists for Serialization only", true)]
        public DatabaseVersion()
        {
            HookupEvents();

            IsEditable = false;
        }

        public DatabaseVersion(int versionId, DateTime created)
        {
            VersionNumber = versionId;
            Created = created;
            _isSaved = false;
            IsEditable = false;

            HookupEvents();
        }

        private void HookupEvents()
        {
            _statements[SqlStatementType.PreDeployment].PropertyChanged += SqlStatementPropertyChanged;
            _statements[SqlStatementType.Backfill].PropertyChanged += SqlStatementPropertyChanged;
            _statements[SqlStatementType.PostDeployment].PropertyChanged += SqlStatementPropertyChanged;
        }

        [XmlAttribute("Id")]
        public int VersionNumber
        {
            get
            {
                return _versionNumber;
            }
            set
            {
                if (_versionNumber != value)
                {
                    _versionNumber = value;
                    FirePropertyChangedEvent(x => x.VersionNumber, true);
                }
            }

        }

        [XmlAttribute("Created")]
        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                if (_created != value)
                {
                    _created = value;
                    FirePropertyChangedEvent(x => x.Created, true);
                }
            }
        }

        [XmlAttribute("CardNumber")]
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    FirePropertyChangedEvent(x => x.CardNumber, true);
                }
            }
        }

        [XmlAttribute("Description")]
        public string Description
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

        [XmlElement("PreDeploymentSqlStatement")]
        public virtual SqlStatementCollection PreDeploymentStatements
        {
            get { return _statements[SqlStatementType.PreDeployment]; }
            set
            {
                if (_statements[SqlStatementType.PreDeployment] != value)
                {
                    _statements[SqlStatementType.PreDeployment].PropertyChanged -= SqlStatementPropertyChanged;
                    _statements[SqlStatementType.PreDeployment] = value;
                    _statements[SqlStatementType.PreDeployment].PropertyChanged += SqlStatementPropertyChanged;
                    FirePropertyChangedEvent(x => x.PreDeploymentStatements, true);
                }
            }
        }

        #region Used for Serialization Only
        /// <summary>
        /// DO NOT USE. Used for backwards compatiblity only
        /// </summary>
        [XmlElement("SqlStatement")]
        public virtual SqlStatementCollection Statements
        {
            get { return _statements[SqlStatementType.PreDeployment]; }
            set
            {
                if (_statements[SqlStatementType.PreDeployment] != value)
                {
                    _statements[SqlStatementType.PreDeployment].PropertyChanged -= SqlStatementPropertyChanged;
                    _statements[SqlStatementType.PreDeployment] = value;
                    _statements[SqlStatementType.PreDeployment].PropertyChanged += SqlStatementPropertyChanged;
                    FirePropertyChangedEvent(x => x.Statements, true);
                }
            }
        }

        public virtual bool ShouldSerializeStatements()
        {
            return false;
        }
        #endregion

        [XmlElement("BackfillSqlStatement")]
        public virtual SqlStatementCollection BackfillStatements
        {
            get { return _statements[SqlStatementType.Backfill]; }
            set
            {
                if (_statements[SqlStatementType.Backfill] != value)
                {
                    _statements[SqlStatementType.Backfill].PropertyChanged -= SqlStatementPropertyChanged;
                    _statements[SqlStatementType.Backfill] = value;
                    _statements[SqlStatementType.Backfill].PropertyChanged += SqlStatementPropertyChanged;
                    FirePropertyChangedEvent(x => x.BackfillStatements, true);
                }
            }
        }

        [XmlElement("PostDeploymentSqlStatement")]
        public virtual SqlStatementCollection PostDeploymentStatements
        {
            get { return _statements[SqlStatementType.PostDeployment]; }
            set
            {
                if (_statements[SqlStatementType.PostDeployment] != value)
                {
                    _statements[SqlStatementType.PostDeployment].PropertyChanged -= SqlStatementPropertyChanged;
                    _statements[SqlStatementType.PostDeployment] = value;
                    _statements[SqlStatementType.PostDeployment].PropertyChanged += SqlStatementPropertyChanged;
                    FirePropertyChangedEvent(x => x.PostDeploymentStatements, true);
                }
            }
        }

        [XmlAttribute("IsBaseline")]
        public virtual bool IsBaseline
        {
            get { return _isBaseline; }
            set
            {
                _isBaseline = value;

                if (_isBaseline != value)
                {
                    _isBaseline = value;
                    FirePropertyChangedEvent(x => x.IsBaseline, true);
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

                    PreDeploymentStatements.ForEach(s => s.IsEditable = value);
                    BackfillStatements.ForEach(s => s.IsEditable = value);
                    PostDeploymentStatements.ForEach(s => s.IsEditable = value);

                    FirePropertyChangedEvent(x => x.IsEditable, true);
                }
            }
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
                    FirePropertyChangedEvent(x => x.IsSaved, false);
                }
            }
        }

        public virtual CompiledVersionSql CompileSql(string databasePrefix, SqlStatementType sqlStatementType,bool includeHistoryStatements)
        {
            var compiledSql = new CompiledVersionSql(VersionNumber, sqlStatementType);

            foreach (var statement in _statements[sqlStatementType])
            {

                var compiledUpgradeSql = new CompiledUpgradeSql(statement.UpgradeSQL,
                                                                databasePrefix,
                                                                statement.Id,
                                                                sqlStatementType,
                                                                VersionNumber,
                                                                includeHistoryStatements);
                compiledSql.AddUpgrade(compiledUpgradeSql);
            }

            foreach (var statement in _statements[sqlStatementType].Reverse())
            {
                var compiledRollbackSql = new CompiledRollbackSql(statement.RollbackSQL,
                                                                  databasePrefix,
                                                                  statement.Id,
                                                                  sqlStatementType,
                                                                  VersionNumber,
                                                                  includeHistoryStatements);

                compiledSql.AddRollback(compiledRollbackSql);
            }

            return compiledSql;
        }

        public static DatabaseVersion Coalesce(DatabaseVersion databaseVersion)
        {
            return databaseVersion ?? new EmptyDatabaseVersion();
        }

        public virtual bool CanBeBuilt()
        {
            return true;
        }

        public void MarkAsSaved()
        {
            IsSaved = true;
            BackfillStatements.MarkAsSaved();
            PreDeploymentStatements.MarkAsSaved();
            PostDeploymentStatements.MarkAsSaved();
        }

        public virtual bool HasStatements
        {
            get
            {
                return PreDeploymentStatements.Count > 0 ||
                       BackfillStatements.Count > 0 ||
                       PostDeploymentStatements.Count > 0;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SqlStatementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetIsSavedPropertyName() && !((ISqlStatementCollection)sender).IsSaved)
            {
                IsSaved = false;
            }
        }

        private string GetIsSavedPropertyName()
        {
            return ((Expression<Func<ISqlStatementCollection, object>>)(x => x.IsSaved)).GetMemberName();
        }

        private void FirePropertyChangedEvent(Expression<Func<DatabaseVersion, object>> propertyNameFunc, bool makeNotSaved)
        {
            if (makeNotSaved)
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