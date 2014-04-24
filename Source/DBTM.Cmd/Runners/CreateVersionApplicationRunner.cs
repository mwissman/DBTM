using System;
using DBTM.Cmd.Arguments;
using DBTM.Domain;

namespace DBTM.Cmd.Runners
{
    public class CreateVersionApplicationRunner : IApplicationRunner<ICreateVersionArguments>
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IMigrator _migrator;

        public CreateVersionApplicationRunner(IDatabaseRepository databaseRepository, IMigrator migrator)
        {
            _databaseRepository = databaseRepository;
            _migrator = migrator;
        }

        public ApplicationRunnerResult Run(ICreateVersionArguments arguments)
        {
            var database = _databaseRepository.Load(arguments.DatabaseSchemaFilePath);
            database.AddChangeset();
            _migrator.EnsureStatementsHaveIds(database);
            _databaseRepository.Save(database,arguments.DatabaseSchemaFilePath);

            return new ApplicationRunnerResult(true,"Create Version succeeded.");
        }
    }
}