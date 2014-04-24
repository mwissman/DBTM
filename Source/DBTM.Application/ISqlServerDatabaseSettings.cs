namespace DBTM.Application
{
    public interface ISqlServerDatabaseSettings
    {
        string DatabaseName { get; }
        string Server { get; }
        string UserToReceiveAccess { get; }
        string DatabaseFilePath { get; }
        string AdminConnectionString { get;  }
        string Password { get; }
        string ConnectionString { get; }
        string CrossDatabaseNamePrefix { get;  }
    }
}