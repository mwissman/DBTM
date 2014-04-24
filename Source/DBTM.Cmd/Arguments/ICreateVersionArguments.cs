namespace DBTM.Cmd.Arguments
{
    public interface ICreateVersionArguments : IArguments
    {
        string DatabaseSchemaFilePath { get; }
    }
}