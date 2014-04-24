using System.Text.RegularExpressions;

namespace DBTM.Domain.Entities
{
    public abstract class CompiledSqlStatementBase
    {
        protected string ProcessCrossDatabasePrefix(string sql, string databasePrefix)
        {
            string nonNullSql = sql ?? string.Empty;
            return Regex.Replace(nonNullSql, @"\{dbx\:(\w+)\}", m => string.Format("{0}{1}", databasePrefix, m.Groups[1].Value));
        }
    }
}