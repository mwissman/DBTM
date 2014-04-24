namespace DBTM.Cmd.Arguments
{
    public class CompileScriptsArguments : ICompileScriptsArguments
    {
        private readonly string[] _arguments;

        private const string COMPILEDSCRIPTDIRECTORY_KEY = "CompiledScriptDirectory";
        private const string DATABASE_FILE_PATH_KEY = "databaseFilePath";

        public CompileScriptsArguments(string[] arguments)
        {
            _arguments = arguments;
        }

        public bool HasRequiredArguments
        {
            get
            {
                return _arguments.HasArgumentValue(COMPILEDSCRIPTDIRECTORY_KEY) &&
                       _arguments.HasArgumentValue(DATABASE_FILE_PATH_KEY);
            }
        }

        public virtual string CompiledScriptDirectory
        {
            get { return _arguments.GetArgumentValue(COMPILEDSCRIPTDIRECTORY_KEY); }
        }

        public virtual string DatabaseSchemaFilePath
        {
            get { return _arguments.GetArgumentValue(DATABASE_FILE_PATH_KEY); }
        }

        public override string ToString()
        {
            return string.Format("-{0}={1} ", COMPILEDSCRIPTDIRECTORY_KEY, CompiledScriptDirectory);
        }
    }
}