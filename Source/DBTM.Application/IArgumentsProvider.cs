namespace DBTM.Application
{
    public interface IArgumentsProvider
    {
        bool HasFile { get; }
        string FilePath { get; }
    }
}