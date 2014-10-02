using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application.Commands
{
    public class AddVersionCommand : ICommand
    {
        private readonly IMainWindowView _view;
        private readonly IDatabaseSchemaViewModel _viewModel;
        private readonly IMigrator _migrator;

        public AddVersionCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel, IMigrator migrator)
        {
            _view = view;
            _viewModel = viewModel;
            _migrator = migrator;
        }

        public void Execute(object parameter)
        {
            var database = _viewModel.Database;
            var newVersion = database.AddChangeset();
            
            _migrator.Migrate(database);

            _view.UpdateSelectedVersion(newVersion);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}