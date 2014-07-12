using DBTM.Application;
using DBTM.Cmd.Arguments;
using DBTM.Domain;

namespace DBTM.Cmd.Runners
{
    public class FullBuildApplicationRunner : IApplicationRunner<IFullBuildArguments>
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IDatabaseBuildService _databaseBuildService;
        private readonly ISqlServerDatabaseSettingsBuilder _sqlServerDatabaseSettingsBuilder;

        public FullBuildApplicationRunner(IDatabaseRepository databaseRepository, 
            IDatabaseBuildService databaseBuildService,
            ISqlServerDatabaseSettingsBuilder sqlServerDatabaseSettingsBuilder)
        {
            _databaseRepository = databaseRepository;
            _databaseBuildService = databaseBuildService;
            _sqlServerDatabaseSettingsBuilder = sqlServerDatabaseSettingsBuilder;
        }

        public ApplicationRunnerResult Run(IFullBuildArguments arguments)
        {
            var databaseSettings = _sqlServerDatabaseSettingsBuilder.Build(arguments);
            var database = _databaseRepository.Load(arguments.DatabaseSchemaFilePath);

            DatabaseBuildResult databaseBuildResult = _databaseBuildService.FullBuild(database, databaseSettings);

            return new ApplicationRunnerResult(databaseBuildResult.Succeeded, databaseBuildResult.Message);
        }
    }
}