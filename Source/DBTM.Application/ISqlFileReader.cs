namespace DBTM.Application
{
    public interface ISqlFileReader
    {
        ISqlFileList GetFromDirectoryPath(string directoryPath);
    }
}