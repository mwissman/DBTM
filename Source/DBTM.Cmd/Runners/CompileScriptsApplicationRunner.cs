using System.Windows.Input;
using DBTM.Cmd.Arguments;
using DBTM.Domain;

namespace DBTM.Cmd.Runners
{
    public class CompileScriptsApplicationRunner : IApplicationRunner<ICompileScriptsArguments>
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ICommand _compileVersionCommand;

        public CompileScriptsApplicationRunner(IDatabaseRepository databaseRepository, ICommand compileVersionCommand)
        {
           
            _databaseRepository = databaseRepository;
            _compileVersionCommand = compileVersionCommand;
        }

        public ApplicationRunnerResult Run(ICompileScriptsArguments arguments)
        {
            var database = _databaseRepository.Load(arguments.DatabaseSchemaFilePath);

            if (database.Versions.Count == 0)
            {
                return new ApplicationRunnerResult(false, "No versions exist in this schema");
            }

            _compileVersionCommand.Execute(database);

            return new ApplicationRunnerResult(true,"Compile Scripts completed successfully.");
        }
    }
}