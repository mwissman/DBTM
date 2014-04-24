using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application.Commands
{
    public class CloseWindowCommand : ICommand
    {
        private readonly IMainWindowView _view;
        private readonly IDatabaseSchemaViewModel _viewModel;

        public CloseWindowCommand(IMainWindowView view, IDatabaseSchemaViewModel viewModel)
        {
            _view = view;
            _viewModel = viewModel;
        }
        
        public void Execute(object parameter)
        {
            var canCloseWindow = true;
            if (!_viewModel.Database.IsSaved)
            {
                canCloseWindow = _view.AskUserChangesShouldBeAbandoned();
            }

            CanCloseWindow = canCloseWindow;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanCloseWindow { get; private set; }
    }
}