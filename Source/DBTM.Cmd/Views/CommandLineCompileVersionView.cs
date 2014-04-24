using System;
using DBTM.Application.Views;
using DBTM.Cmd.Arguments;

namespace DBTM.Cmd.Views
{
    public class CommandLineCompileVersionView: ICompileVersionView
    {
        private readonly ICompileScriptsArguments _arguments;

        public CommandLineCompileVersionView(ICompileScriptsArguments arguments)
        {
            _arguments = arguments;
        }

        public void DisplayStatusMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string AskUserForCrossDatabasePrefix()
        {
            return String.Empty;
        }

        public string AskUserForCompiledSqlFolderPath()
        {
            return _arguments.CompiledScriptDirectory;
        }
    }
}