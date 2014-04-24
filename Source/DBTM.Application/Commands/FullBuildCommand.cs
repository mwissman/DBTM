using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;

namespace DBTM.Application.Commands
{
    public class FullBuildCommand : ICommand
    {
        private readonly IMainWindowView _view;
        private readonly IDatabaseSchemaViewModel _viewModel;
        private readonly IDatabaseBuildService _databaseBuildService;
        private FullBuildDialogResults _previousFullBuildSettings;

        public FullBuildCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel, IDatabaseBuildService databaseBuildService)
        {
            _view = view;
            _viewModel = viewModel;
            _databaseBuildService = databaseBuildService;
        }

        public void Execute(object parameter)
        {   
            if (!_viewModel.Database.CanFullBuild)
            {
                _view.DisplayError("Cannot run Full Build on an empty database. Please set a database name.");
                return;
            }

            FullBuildDialogResults settings = null;
            if (_previousFullBuildSettings==null)
            {
                settings = _view.AskUserForFullBuildParameters(_viewModel.Database.DbName);
            }
            else
            {
                settings = _view.AskUserForFullBuildParameters(_viewModel.Database.DbName,_previousFullBuildSettings);
            }
            
            _previousFullBuildSettings = settings;
            
            if (!settings.WasCanceled)
            {                
                var buildResult = _databaseBuildService.FullBuild(_viewModel.Database, settings.ToSqlServerSettings());
                _view.ShowBuildResultMessage(buildResult.Message, _viewModel.Database.DbName);
            } 
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}