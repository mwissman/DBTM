using System;

namespace DBTM.Application
{
    public class SqlServerDatabaseSettings : ISqlServerDatabaseSettings
    {
        public SqlServerDatabaseSettings(string databaseName, string server, string userToReceiveAccess, string databaseFilePath, string password, string crossDatabasePrefix)
        {
            DatabaseName = databaseName;
            Server = server;
            UserToReceiveAccess = userToReceiveAccess;
            DatabaseFilePath = databaseFilePath;
            Password = password;
            CrossDatabaseNamePrefix = crossDatabasePrefix;
        }

        public string DatabaseName { get; protected set; }
        public string Server { get; protected set; }
        public string UserToReceiveAccess { get; protected set; }
        public string DatabaseFilePath { get; protected set; }
        public string Password { get; protected set; }
        public string CrossDatabaseNamePrefix { get; protected set; }

        public string ConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Pooling=false;",
                                   Server,
                                   DatabaseName,
                                   UserToReceiveAccess,
                                   Password);
            }
        }

        public string AdminConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog=Master;User Id={1};Password={2};Pooling=false;",
                                     Server,
                                     UserToReceiveAccess,
                                     Password);
            }
        }   
    }
}