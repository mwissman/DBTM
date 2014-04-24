namespace DBTM.Application
{
    public interface ISqlServerDatabase
    {
        void Initialize(ISqlServerDatabaseSettings settings);
    }
}