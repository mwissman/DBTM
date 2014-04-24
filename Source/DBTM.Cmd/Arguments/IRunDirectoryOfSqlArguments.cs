namespace DBTM.Cmd.Arguments
{
    public interface IRunDirectoryOfSqlArguments : IArguments
    {
        string ScriptDirectoryPath { get; }
        string[] RawArguments { get; }
    }
}