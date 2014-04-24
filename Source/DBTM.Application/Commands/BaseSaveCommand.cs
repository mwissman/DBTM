using System;
using System.Linq.Expressions;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application.Commands
{
    public abstract class BaseSaveCommand : ICommand
    {
        protected readonly IMainWindowView _view;
        protected readonly IDatabaseSchemaViewModel _viewModel;

        protected BaseSaveCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel)
        {
            _view = view;
            _viewModel = viewModel;
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _viewModel.Database.IsEditable;
        }
       
        public abstract void Execute(object parameter);

        protected AskForFilePathResult AskUserForFilePathAndUpdateViewModel()
        {
            var filepath = _view.AskUserForNewFilePath();

            if (string.IsNullOrEmpty(filepath))
            {
                return new AskForFilePathResult(){DidUserProvidePath = false, FilePath = string.Empty};
            }

            _viewModel.Settings.DatabaseFilePath = filepath;

            return new AskForFilePathResult() { DidUserProvidePath = true, FilePath = filepath }; ;
        }

        protected class AskForFilePathResult
        {
            public bool DidUserProvidePath { get; set;}
            public string FilePath { get; set; }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Expression<Func<DatabaseSchemaViewModel, object>> propertyNameFunc = vm => vm.Database;
            var databasePropertyName = propertyNameFunc.GetMemberName();

            if (e.PropertyName==databasePropertyName)
            {
                if (CanExecuteChanged!=null)
                {
                    CanExecuteChanged.Invoke(this,new EventArgs());
                }
            }
        }
    }
}