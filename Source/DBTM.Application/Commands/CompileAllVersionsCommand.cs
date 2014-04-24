using System;
using System.Windows.Input;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;

namespace DBTM.Application.Commands
{
    public class CompileAllVersionsCommand : ICommand
    {
        private readonly ICompileVersionView _view;

        private readonly IDatabaseSchemaViewModel _viewModel;
        private readonly ICompiler _compiler;

        public CompileAllVersionsCommand(ICompileVersionView view, IDatabaseSchemaViewModel viewModel, ICompiler compiler)
        {
            _view = view;
            _viewModel = viewModel;
            _compiler = compiler;
        }

        public void Execute(object parameter)
        {
            var compileScriptPath = _view.AskUserForCompiledSqlFolderPath();

            if (!string.IsNullOrEmpty(compileScriptPath))
            {
                _compiler.CompileAllVersions(_viewModel.Database, compileScriptPath);
                _view.DisplayStatusMessage("Compile Sql for all Versions completed successfully");
            }
            else
            {
                _view.DisplayStatusMessage("Compile Sql for all Versions was canceled");
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}