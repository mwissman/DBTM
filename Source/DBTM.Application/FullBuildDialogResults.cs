namespace DBTM.Application
{
    public class FullBuildDialogResults
    {
        public bool WasCanceled { get; set; }
        public string DatabaseFilePath { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabaseServer { get; set; }
        public string Password { get; set; }
        public string CrossDatabaseNamePrefix { get; set; }

        public ISqlServerDatabaseSettings ToSqlServerSettings()
        {
            return new SqlServerDatabaseSettings(DatabaseName, DatabaseServer, DatabaseUsername, DatabaseFilePath, Password, CrossDatabaseNamePrefix);
        }
    }
}