namespace DBTM.Application
{
    public class SetConnectionStringResult
    {
        public SetConnectionStringResult(bool wasCancel, string connectionString)
        {
            WasCancel = wasCancel;
            ConnectionString = connectionString;
        }

        public bool WasCancel { get; private set; }
        public string ConnectionString { get; private set; }
    }
}