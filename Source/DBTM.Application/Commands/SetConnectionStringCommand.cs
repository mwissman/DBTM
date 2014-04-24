using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;

namespace DBTM.Application.Commands
{
    public class SetConnectionStringCommand : ICommand
    {
        private readonly IMainWindowView _view;
        private readonly ITestSqlServerConnectionStrings _connectionStringTester;
        private readonly IDatabaseSchemaViewModel _viewModel;

        public SetConnectionStringCommand(IMainWindowView view, ITestSqlServerConnectionStrings connectionStringTester, IDatabaseSchemaViewModel viewModel)
        {
            _view = view;
            _connectionStringTester = connectionStringTester;
            _viewModel = viewModel;
        }

        public void Execute(object parameter)
        {
            var result = _view.AskUserForConnectionString();

            if (!result.WasCancel)
            {
                if (_connectionStringTester.IsValid(result.ConnectionString))
                {
                    _viewModel.Settings.ConnectionString = result.ConnectionString;
                }
                else
                {
                    _view.DisplayError("Invalid Connection String!");
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}