using System;

namespace DBTM.Application.SQL
{
    public class SqlCommandException : Exception
    {
        public string FailureText { get; private set; }
        public string ConnectionString { get; private set; }

        public SqlCommandException(string commandText, string connectionString, string message, Exception exception)
            : base(message, exception)
        {
            FailureText = commandText;
            ConnectionString = connectionString;
        }
    }

    public class SqlCommandDirectoryNotFoundException : Exception
    {
        public SqlCommandDirectoryNotFoundException(string message) : base(message)
        {
        }
    }
}