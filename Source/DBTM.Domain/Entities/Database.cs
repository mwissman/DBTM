using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace DBTM.Domain.Entities
{
    [XmlRoot("Database")]
    public class Database : IEntitySavedStateMonitor, INotifyPropertyChanged
    {
        private DatabaseVersionCollection _databaseVersions = new DatabaseVersionCollection();
        private string _dbName;

        [XmlIgnore]
        protected bool _isSaved = true;

        private bool _disableHistoryTracking;

        [Obsolete("This exists only for serialization", true)]
        public Database()
        {
            _databaseVersions.PropertyChanged += DatabaseVersionsPropertyChanged;
        }

        public Database(string dbName)
        {
            DbName = dbName;
            _isSaved = false;
            _databaseVersions.PropertyChanged += DatabaseVersionsPropertyChanged;
        }

        [XmlAttribute("Name")]
        public virtual string DbName
        {
            get { return _dbName; }
            set
            {
                if (_dbName != value)
                {
                    _dbName = value;
                    FirePropertyChangedEvent(x => x.DbName);
                }

            }
        }

        [XmlAttribute("DisableHistoryTracking")]
        public bool DisableHistoryTracking
        {
            get { return _disableHistoryTracking; }
            set
            {
                if (_disableHistoryTracking != value)
                {
                    _disableHistoryTracking = value;
                    FirePropertyChangedEvent(x => x.DisableHistoryTracking,true);
                }
            }
        }

        [XmlElement("Version")]
        public DatabaseVersionCollection Versions
        {
            get { return _databaseVersions; }
            set
            {
                if (_databaseVersions != value)
                {
                    _databaseVersions.PropertyChanged -= DatabaseVersionsPropertyChanged;
                    _databaseVersions = value;
                    _databaseVersions.PropertyChanged += DatabaseVersionsPropertyChanged;
                    FirePropertyChangedEvent(x => x.Versions);
                }
            }
        }

        [XmlIgnore]
        public virtual bool CanFullBuild { get { return true; } }

        [XmlIgnore]
        public virtual bool IsSaved
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

        [XmlIgnore]
        public virtual bool IsEditable
        {
            get { return true; }
        }

        public virtual int BaseLineVersionNumber { get { return Versions.Where(v => v.IsBaseline).Select(v => v.VersionNumber).DefaultIfEmpty(int.MaxValue).FirstOrDefault(); } }

        public virtual bool HasBaseline { get { return Versions.Any(v => v.IsBaseline); } }

        public DatabaseVersion this[int versionId]
        {
            get
            {
                var version = Versions.FirstOrDefault(v => v.VersionNumber == versionId);
                if (version == null)
                {
                    throw new IndexOutOfRangeException();
                }
                return version;
            }
        }

        public virtual DatabaseVersion AddChangeset()
        {
            Versions.ForEach(v => v.IsEditable = false);

            int nextVersionNumber = Versions.Count > 0 ? Versions.Max(v => v.VersionNumber) + 1 : 1;

            var hightestVersion = Versions.OrderByDescending(v => v.VersionNumber).Take(1).FirstOrDefault();


            var newVersionIsTheBaseline = !HasBaseline && !DisableHistoryTracking;

            if ((hightestVersion == null) || (hightestVersion.HasStatements))
            {
                var databaseVersion = new DatabaseVersion(nextVersionNumber, DateTime.Now) { IsEditable = true, IsBaseline = newVersionIsTheBaseline };
                Versions.Add(databaseVersion);
                FirePropertyChangedEvent(x=>x.HasBaseline);
                FirePropertyChangedEvent(x=>x.BaseLineVersionNumber);

                return databaseVersion;
            }

            hightestVersion.VersionNumber = nextVersionNumber;
            hightestVersion.Created = DateTime.Now;
            hightestVersion.IsEditable = true;
            hightestVersion.IsBaseline = newVersionIsTheBaseline;

            return hightestVersion;
        }

        public virtual void MarkAsSaved()
        {
            IsSaved = true;
            Versions.MarkAsSaved();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(Expression<Func<Database, object>> propertyNameFunc, bool makeNotSaved=false)
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

        private void DatabaseVersionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetIsSavedPropertyName() && !((DatabaseVersionCollection)sender).IsSaved)
            {
                IsSaved = false;
            }
        }

        private string GetIsSavedPropertyName()
        {
            return ((Expression<Func<Database, object>>)(x => x.IsSaved)).GetMemberName();
        }

        public virtual ICompiledSql CompileAllVersions(ICompiledSql compiledHistorySql)
        {
            var compiledDatabaseSql = new CompiledDatabaseSql(DbName, compiledHistorySql);
            foreach (var version in Versions)
            {
                bool includeHistory = version.VersionNumber >= BaseLineVersionNumber;
                var preDeployCompiledSql = version.CompileSql("", SqlStatementType.PreDeployment, includeHistory);
                var postDeployCompiledSql = version.CompileSql("", SqlStatementType.PostDeployment, includeHistory);
                compiledDatabaseSql.AddVersion(preDeployCompiledSql, postDeployCompiledSql);
            }

            return compiledDatabaseSql;
        }

    }
}