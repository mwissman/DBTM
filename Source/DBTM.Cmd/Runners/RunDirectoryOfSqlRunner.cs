using System;
using System.Collections.Generic;
using DBTM.Application;
using DBTM.Application.SQL;
using DBTM.Cmd.Arguments;

namespace DBTM.Cmd.Runners
{
    public class RunDirectoryOfSqlRunner : IRunDirectoryOfSqlRunner
    {
        private readonly ISqlRunner _sqlRunner;
        private readonly ISqlServerDatabaseSettingsBuilder _sqlServerDatabaseSettings;
        private readonly ISqlFileReader _sqlFileReader;

        public RunDirectoryOfSqlRunner(ISqlRunner sqlRunner,
            ISqlServerDatabaseSettingsBuilder sqlServerDatabaseSettings, 
            ISqlFileReader sqlFileReader)
        {
            _sqlRunner = sqlRunner;
            _sqlServerDatabaseSettings = sqlServerDatabaseSettings;
            _sqlFileReader = sqlFileReader;
        }

        public ApplicationRunnerResult Run(IRunDirectoryOfSqlArguments arguments)
        {
            var sqlFiles = _sqlFileReader.GetFromDirectoryPath(arguments.ScriptDirectoryPath);
            string adminConnectionString = _sqlServerDatabaseSettings.Build(new RunDirectoryArguments(arguments.RawArguments)).ConnectionString;

            foreach (var sqlFile in sqlFiles.Files)
            {
                Console.WriteLine("FileName: {0}" ,sqlFile.FileName);

                try
                {
                    _sqlRunner.RunAdminScripts(new List<string>{ sqlFile.Contents}, adminConnectionString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FileName: {0}", sqlFile.FileName);
                    Console.WriteLine("Error: {0}", ex);
                    throw;
                }
            }


            return new ApplicationRunnerResult(true, "Scripts Run");
        }

        private class RunDirectoryArguments : IFullBuildArguments
        {
            private readonly string[] _args;

            private const string DATABASENAME_VALUE = "databaseName";
            private const string SERVER_VALUE = "server";
            private const string USERNAME_VALUE = "userName";
            private const string PASSWORD = "password";

            public RunDirectoryArguments(string[] args)
            {
                _args = args;
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
                get { return string.Empty; }
            }

            public string DatabaseSchemaFilePath
            {
                get { return string.Empty; }
            }

            public string Password
            {
                get { return _args.GetArgumentValue(PASSWORD); }
            }

            public string CrossDatabaseNamePrefix
            {
                get { return string.Empty; }
            }

            public bool HasRequiredArguments
            {
                get
                {
                    return _args.HasArgumentValue(DATABASENAME_VALUE) &&
                           _args.HasArgumentValue(SERVER_VALUE) &&
                           _args.HasArgumentValue(USERNAME_VALUE) &&
                           _args.HasArgumentValue(PASSWORD);
                }

            }

            public override string ToString()
            {
                return string.Format("-{0}={1} -{2}={3} -{4}={5} -{6}={7}",
                                     DATABASENAME_VALUE,
                                     DatabaseName,
                                     SERVER_VALUE,
                                     Server,
                                     USERNAME_VALUE,
                                     UserName,
                                     PASSWORD,
                                     Password);
            }

        }
    }
}