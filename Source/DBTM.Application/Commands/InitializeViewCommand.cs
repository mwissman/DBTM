using System;
using System.Windows.Input;
using DBTM.Application.Views;

namespace DBTM.Application.Commands
{
    public class InitializeViewCommand : ICommand
    {
        private readonly IMainWindowView _view;
        private readonly IArgumentsProvider _argumentsProvider;
        private readonly OpenDatabaseCommand _openDatabaseCommand;

        public InitializeViewCommand(IMainWindowView view,IArgumentsProvider argumentsProvider, OpenDatabaseCommand openDatabaseCommand)
        {
            _view = view;
            _argumentsProvider = argumentsProvider;
            _openDatabaseCommand = openDatabaseCommand;
        }

        public void Execute(object parameter)
        {
            if (_argumentsProvider.HasFile)
            {
                if (_openDatabaseCommand.CanExecute(null))
                {
                    _openDatabaseCommand.Execute(_argumentsProvider.FilePath);
                }
            }

            _view.Show();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}