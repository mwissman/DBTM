namespace DBTM.Cmd.Arguments
{
    public interface IArgumentsFactory
    {
        IArguments Create(string[] arguments);
    }
}