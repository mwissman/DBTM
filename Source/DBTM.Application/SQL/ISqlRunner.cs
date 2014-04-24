using System.Collections.Generic;
using DBTM.Domain.Entities;

namespace DBTM.Application.SQL
{
    public interface ISqlRunner
    {
        void RunAdminScripts(IList<string> sqlStatements, string adminConnectionString);
        
        void RunUpgrade(ICompiledSql compiledSql, string connectionString);
        void RunRollback(ICompiledSql compiledSql, string connectionString);
    }
}