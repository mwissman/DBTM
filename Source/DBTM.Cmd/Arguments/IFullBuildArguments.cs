namespace DBTM.Cmd.Arguments
{
    public interface IFullBuildArguments : IArguments
    {
        string DatabaseName { get; }
        string Server { get; }
        string UserName { get; }
        string DataFilePath { get; }
        string DatabaseSchemaFilePath { get; }
        string Password { get; }
        string CrossDatabaseNamePrefix { get; }
    }
}