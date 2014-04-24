using System;
using System.Windows.Input;
using DBTM.Application.Views;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class CompileVersionCommand : ICommand
    {
        private readonly ICompileVersionView _view;
        private readonly ICompiler _compiler;

        public CompileVersionCommand(ICompileVersionView view, ICompiler compiler)
        {
            _view = view;
            _compiler = compiler;
        }

        public void Execute(object parameter)
        {
            var database = parameter as Database;

            if (database != null)
            {
                var compileScriptPath = _view.AskUserForCompiledSqlFolderPath();
                
                if (!string.IsNullOrEmpty(compileScriptPath))
                {
                    var prefix = _view.AskUserForCrossDatabasePrefix();

                    _compiler.CompileLatestVersion(database,compileScriptPath,prefix);

                    _view.DisplayStatusMessage("Compile Sql Version completed successfully");
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