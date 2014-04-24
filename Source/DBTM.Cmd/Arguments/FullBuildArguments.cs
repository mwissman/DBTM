namespace DBTM.Cmd.Arguments
{
    public class FullBuildArguments : IFullBuildArguments
    {
        private readonly string[] _args;
        private const string DATABASENAME_VALUE = "databaseName";
        private const string SERVER_VALUE = "server";
        private const string USERNAME_VALUE = "userName";
        private const string DATA_FILE_PATH = "dataFilePath";
        private const string DATABASE_FILE_PATH = "databaseFilePath";
        private const string PASSWORD = "password";
        private const string CROSS_DATABASE_NAME_PREFIX = "crossDatabaseNamePrefix";

        public FullBuildArguments(string[] arguments)
        {
            _args = arguments;
        }

        public string DatabaseName
        {
            get { return _args.GetArgumentValue(DATABASENAME_VALUE); }
        }
        public string Server
        {
            get { return _args.GetArgumentValue(SERVER_VALUE); }
        }
        public string UserName
        {
            get { return _args.GetArgumentValue(USERNAME_VALUE); }
        }
        public string DataFilePath
        {
            get { return _args.GetArgumentValue(DATA_FILE_PATH); }
        }

        public string DatabaseSchemaFilePath
        {
            get { return _args.GetArgumentValue(DATABASE_FILE_PATH); }
        }

        public string Password
        {
            get { return _args.GetArgumentValue(PASSWORD); }
        }

        public string CrossDatabaseNamePrefix
        {
            get { return _args.GetArgumentValue(CROSS_DATABASE_NAME_PREFIX) ?? string.Empty; }
        }

        public bool HasRequiredArguments
        {
            get
            {
                return _args.HasArgumentValue(DATABASENAME_VALUE) &&
                       _args.HasArgumentValue(SERVER_VALUE) &&
                       _args.HasArgumentValue(USERNAME_VALUE) &&
                       _args.HasArgumentValue(DATA_FILE_PATH) &&
                       _args.HasArgumentValue(DATABASE_FILE_PATH) &&
                       _args.HasArgumentValue(PASSWORD);
            }

        }

        public override string ToString()
        {
            return string.Format("-{0}={1} -{2}={3} -{4}={5} -{6}={7} -{8}={9} -{10}={11} -{12}={13}",
                                 DATABASENAME_VALUE,
                                 DatabaseName,
                                 SERVER_VALUE,
                                 Server,
                                 DATA_FILE_PATH,
                                 DataFilePath,
                                 DATABASE_FILE_PATH,
                                 DatabaseSchemaFilePath,
                                 USERNAME_VALUE,
                                 UserName,
                                 PASSWORD,
                                 Password,
                                 CROSS_DATABASE_NAME_PREFIX,
                                 CrossDatabaseNamePrefix);
        }

    }
}