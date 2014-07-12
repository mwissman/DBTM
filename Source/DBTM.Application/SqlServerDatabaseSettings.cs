using System;

namespace DBTM.Application
{
    public class SqlServerDatabaseSettings : ISqlServerDatabaseSettings
    {
        private readonly IAuthenticantion _authentication;

        public SqlServerDatabaseSettings(string databaseName, string server, string userToReceiveAccess, string databaseFilePath, string password, string crossDatabasePrefix)
        :this(databaseName,server,databaseFilePath,crossDatabasePrefix,new UsernamePassword(userToReceiveAccess,password))
        {
            
        }

        public SqlServerDatabaseSettings(string databaseName, string server, string databaseFilePath,
            string crossDatabasePrefix, IAuthenticantion authentication)
        {
            _authentication = authentication;
            DatabaseName = databaseName;
            Server = server;
            DatabaseFilePath = databaseFilePath;
            CrossDatabaseNamePrefix = crossDatabasePrefix;

        }

        public string DatabaseName { get; protected set; }
        public string Server { get; protected set; }
        public string DatabaseFilePath { get; protected set; }
        public string CrossDatabaseNamePrefix { get; protected set; }

        public string ConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog={1};{2}Pooling=false;",
                                   Server,
                                   DatabaseName,
                                   _authentication.ToConnectionStringFragment());
            }
        }

        public string AdminConnectionString
        {
            get
            {
                return string.Format("Data Source={0};Initial Catalog=Master;{1}Pooling=false;",
                                     Server,
                                     _authentication.ToConnectionStringFragment());
            }
        }   
    }
}