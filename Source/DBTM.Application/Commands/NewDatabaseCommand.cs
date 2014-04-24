using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class NewDatabaseCommand : ICommand
    {
        private IMainWindowView _view;
        private IDatabaseSchemaViewModel _viewModel;

        public NewDatabaseCommand(IMainWindowView view,IDatabaseSchemaViewModel viewModel)
        {
            _viewModel = viewModel;
            _view = view;
        }

        public void Execute(object parameter)
        {

            var name = _view.AskUserForNewDatabasename();
            if (!string.IsNullOrEmpty(name))
            {
                _viewModel.Database = new Database(name);
                _viewModel.Settings.DatabaseFilePath = string.Empty;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}