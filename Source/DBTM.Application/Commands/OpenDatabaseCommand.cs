using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class OpenDatabaseCommand : ICommand
    {
        private readonly ICanOpenDatabasesView _view;
        private readonly IDatabaseSchemaViewModel _viewModel;
        private readonly IDatabaseRepository _databaseRepository;

        public OpenDatabaseCommand(ICanOpenDatabasesView view, 
            IDatabaseSchemaViewModel viewModel, 
            IDatabaseRepository databaseRepository)
        {
            _view = view;
            _viewModel = viewModel;
            _databaseRepository = databaseRepository;            
        }

        public virtual void Execute(object parameter)
        {
            string filePath = (parameter is string) ? (string)parameter : _view.OpenFile();

            if (!string.IsNullOrEmpty(filePath) && ShouldClose())
            {
                _viewModel.Settings.DatabaseFilePath = filePath;
                Database database = _databaseRepository.Load(filePath);
                _viewModel.Database = database;

            }
        }

        public virtual bool ShouldClose()
        {
            if (!_viewModel.Database.IsSaved)
            {
                return _view.AskUserChangesShouldBeAbandoned();
            }
            return true;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}