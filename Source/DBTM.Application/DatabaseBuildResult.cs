namespace DBTM.Application
{
    public class DatabaseBuildResult
    {
        public DatabaseBuildResult(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
    }
}