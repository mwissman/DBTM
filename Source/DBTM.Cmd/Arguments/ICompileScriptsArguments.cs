namespace DBTM.Cmd.Arguments
{
    public interface ICompileScriptsArguments : IArguments
    {
        string CompiledScriptDirectory { get; }
        string DatabaseSchemaFilePath { get; }
    }
}