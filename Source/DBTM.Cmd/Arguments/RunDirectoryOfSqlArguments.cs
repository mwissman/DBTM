using System;

namespace DBTM.Cmd.Arguments
{
    public class RunDirectoryOfSqlArguments : IRunDirectoryOfSqlArguments
    {
        private readonly string[] _arguments;

        private const string SCRIPT_FILE_PATH_KEY = "scriptPath";

        public RunDirectoryOfSqlArguments(string[] arguments)
        {
            _arguments = arguments;
        }

        public bool HasRequiredArguments
        {
            get { return _arguments.HasArgumentValue(SCRIPT_FILE_PATH_KEY); }
        }

        public string ScriptDirectoryPath
        {
            get { return _arguments.GetArgumentValue(SCRIPT_FILE_PATH_KEY); }
        }

        public string[] RawArguments
        {
            get { return _arguments; }
        }
    }
}