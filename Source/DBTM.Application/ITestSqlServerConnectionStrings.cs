namespace DBTM.Application
{
    public interface ITestSqlServerConnectionStrings
    {
        bool IsValid(string connectionString);
    }
}