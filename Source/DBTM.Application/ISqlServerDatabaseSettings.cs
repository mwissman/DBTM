namespace DBTM.Application
{
    public interface ISqlServerDatabaseSettings
    {
        string DatabaseName { get; }
        string Server { get; }
        string DatabaseFilePath { get; }
        string AdminConnectionString { get;  }
        string ConnectionString { get; }
        string CrossDatabaseNamePrefix { get;  }
    }

    public interface IAuthenticantion
    {
        string ToConnectionStringFragment();
    }

    public class UsernamePassword : IAuthenticantion
    {
        private readonly string _username;
        private readonly string _password;

        public UsernamePassword(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string ToConnectionStringFragment()
        {
            return string.Format("User Id={0};Password={1};", _username, _password);
        }
    }
}