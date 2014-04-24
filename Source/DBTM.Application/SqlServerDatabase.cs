using System;
using System.Collections.Generic;
using DBTM.Application.SQL;

namespace DBTM.Application
{
    public class SqlServerDatabase : ISqlServerDatabase
    {
        private readonly ISqlRunner _sqlRunner;
        private readonly ISqlScriptRepository _sqlScriptRepository;

        public SqlServerDatabase(ISqlRunner sqlRunner, ISqlScriptRepository sqlScriptRepository)
        {
            _sqlRunner = sqlRunner;
            _sqlScriptRepository = sqlScriptRepository;
        }

        public void Initialize(ISqlServerDatabaseSettings settings)
        {
            IList<string> sqlStatments = new List<string>();

            sqlStatments.Add(_sqlScriptRepository.LoadDisconnectUser(settings.DatabaseName));
            sqlStatments.Add(_sqlScriptRepository.LoadDropSchema(settings.DatabaseName));
            sqlStatments.Add(_sqlScriptRepository.LoadCreateSchema(settings.DatabaseName, settings.DatabaseFilePath));

            _sqlRunner.RunAdminScripts(sqlStatments, settings.AdminConnectionString);
        }
    }
}