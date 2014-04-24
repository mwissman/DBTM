using DBTM.Application;
using DBTM.Cmd.Arguments;

namespace DBTM.Cmd
{
    public class SqlServerDatabaseSettingsBuilder : ISqlServerDatabaseSettingsBuilder
    {
        public ISqlServerDatabaseSettings Build(IFullBuildArguments fullBuildArguments)
        {
            return new SqlServerDatabaseSettings(fullBuildArguments.DatabaseName,
                                                 fullBuildArguments.Server,
                                                 fullBuildArguments.UserName,
                                                 fullBuildArguments.DataFilePath,
                                                 fullBuildArguments.Password,
                                                 fullBuildArguments.CrossDatabaseNamePrefix);
        }
    }
}