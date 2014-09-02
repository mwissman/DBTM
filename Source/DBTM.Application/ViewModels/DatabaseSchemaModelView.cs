using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using DBTM.Application.Commands;
using DBTM.Domain;
using DBTM.Domain.Entities;

namespace DBTM.Application.ViewModels
{
    public class DatabaseSchemaViewModel : IDatabaseSchemaViewModel
    {
        private readonly Func<Type, ICommand> _commandFactory;
        private Database _database;
        private IApplicationSettings _settings;

        public DatabaseSchemaViewModel(Func<Type,ICommand> commandFactory)
        {
            _commandFactory = commandFactory;
            Database = new EmptyDatabase();
            Settings = new ApplicationSettings()
                           {
                               ConnectionString = Constants.DEFUALT_CONNECTIONSTRING,
                               DatabaseFilePath = Constants.DEFUALT_DATABASE_FILE_PATH
                           };

        }

        public string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public Database Database
        {
            get { return _database; }
            set
            {
                if (_database != value)
                {
                    _database = value;
                    FirePropertyChangedEvent(x => x.Database);
                }
            }
        }

        public IApplicationSettings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    FirePropertyChangedEvent(x=>x.Settings);
                }
            }
        }

        public ICommand OpenDatabaseCommand
        {
            get { return _commandFactory.Invoke(typeof(OpenDatabaseCommand)); }
        }

        public ICommand NewDatabaseCommand
        {
            get { return _commandFactory.Invoke(typeof(NewDatabaseCommand)); }
        }

        public ICommand CloseWindowCommand
        {
            get
            {

                return _commandFactory.Invoke(typeof(CloseWindowCommand));
            }
        }

        public ICommand SaveDatabaseAsCommand
        {
            get { return _commandFactory.Invoke(typeof(SaveDatabaseAsCommand)); }
        }

        public ICommand SaveDatabaseCommand
        {
            get { return _commandFactory.Invoke(typeof(SaveDatabaseCommand)); }
        }

        public ICommand MoveStatementUpCommand
        {
            get { return _commandFactory.Invoke(typeof(MoveStatementUpCommand)); }
        }

        public ICommand MoveStatementDownCommand
        {
            get { return _commandFactory.Invoke(typeof(MoveStatementDownCommand)); }
        }

        public ICommand RemoveStatementCommand
        {
            get { return _commandFactory.Invoke(typeof(RemoveStatementCommand)); }
        }

        public ICommand SetConnectionStringCommand
        {
            get { return _commandFactory.Invoke(typeof(SetConnectionStringCommand)); }
        }

        public ICommand CompileAllCommand
        {
            get { return _commandFactory.Invoke(typeof (CompileAllVersionsCommand)); }
        }

        public ICommand InitializeViewCommand
        {
            get { return _commandFactory.Invoke(typeof(InitializeViewCommand)); }
        }

        public ICommand AboutCommand
        {
            get { return _commandFactory.Invoke(typeof(AboutCommand)); }
        }

        public ICommand FullBuildCommand
        {
            get { return _commandFactory.Invoke(typeof(FullBuildCommand)); }
        }

        public ICommand AddVersionCommand
        {
            get { return _commandFactory.Invoke(typeof(AddVersionCommand)); }
        }

        public ICommand AddStatementCommand
        {
            get { return _commandFactory.Invoke(typeof(AddStatementCommand)); }
        }

        public ICommand CompileVersionCommand
        {
            get { return _commandFactory.Invoke(typeof(CompileVersionCommand)); }
        }
      
        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(Expression<Func<DatabaseSchemaViewModel, object>> propertyNameFunc)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyNameFunc.GetMemberName()));
            }
        }
    }
}