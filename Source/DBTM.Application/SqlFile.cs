namespace DBTM.Application
{
    public class SqlFile : ISqlFile
    {
        public SqlFile(string contents, string fileName)
        {
            Contents = contents;
            FileName = fileName;
        }

        public string Contents { get; private set; }

        public string FileName { get; private set; }
    }
}