using DBTM.Application;
using DBTM.Cmd.Arguments;

namespace DBTM.Cmd
{
    public interface ISqlServerDatabaseSettingsBuilder
    {
        ISqlServerDatabaseSettings Build(IFullBuildArguments fullBuildArguments);
    }
}