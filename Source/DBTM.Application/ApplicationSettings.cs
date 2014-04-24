using System;
using System.ComponentModel;
using System.Linq.Expressions;
using DBTM.Domain;


namespace DBTM.Application
{
    public class ApplicationSettings : IApplicationSettings
    {
        private string _connectionString;
        private string _databaseFilePath;

        public virtual string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if ( _connectionString!=value)
                {
                    _connectionString = value;
                    FirePropertyChangedEvent(x=>x.ConnectionString);
                }
                
            }
        }

        public virtual string DatabaseFilePath
        {
            get { return _databaseFilePath; }
            set
            {
                if (_databaseFilePath != value)
                {
                    _databaseFilePath = value;
                    FirePropertyChangedEvent(x => x.DatabaseFilePath);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(Expression<Func<ApplicationSettings, object>> propertyNameFunc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyNameFunc.GetMemberName()));
            }
        }
    }
}