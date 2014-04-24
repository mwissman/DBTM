namespace DBTM.Cmd.Arguments
{
    public class CreateVersionArguments : ICreateVersionArguments
    {
        private readonly string[] _arguments;

        private const string DATABASE_FILE_PATH_KEY = "databaseFilePath";

        public CreateVersionArguments(string[] arguments)
        {
            _arguments = arguments;
        }

        public bool HasRequiredArguments
        {
            get { return _arguments.HasArgumentValue(DATABASE_FILE_PATH_KEY); }
        }

        public string DatabaseSchemaFilePath
        {
            get { return _arguments.GetArgumentValue(DATABASE_FILE_PATH_KEY); }
        }
    }
}