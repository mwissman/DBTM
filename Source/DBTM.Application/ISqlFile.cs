namespace DBTM.Application
{
    public interface ISqlFile
    {
        string Contents { get; }
        string FileName { get; }
    }
}