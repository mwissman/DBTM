using Autofac;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new XMLSerializer()).
                As<IXMLSerializer>().
                InstancePerLifetimeScope();

            builder.Register(c => new DatabaseRepository(
                                      c.Resolve<IXMLSerializer>())).
                As<IDatabaseRepository>().
                InstancePerLifetimeScope();

            builder.Register(c => new GuidFactory())
                .As<IGuidFactory>()
                .InstancePerLifetimeScope();

//            builder.Register(c => new CombinedMigrator(new EnsureStatementsHaveIds(c.Resolve<IGuidFactory>()),new GenerateDescriptions(),new RemoveAnsiNullStatements()))
            builder.Register(c => new EnsureStatementsHaveIds(c.Resolve<IGuidFactory>()))
                .As<IMigrator>()
                .InstancePerLifetimeScope();
        }
    }

    public class CombinedMigrator : IMigrator
    {
        private readonly EnsureStatementsHaveIds _ensureStatementsHaveIds;
        private readonly GenerateDescriptions _generateDescriptions;
        private readonly RemoveAnsiNullStatements _removeAnsiNullStatements;

        public CombinedMigrator(EnsureStatementsHaveIds ensureStatementsHaveIds, GenerateDescriptions generateDescriptions, RemoveAnsiNullStatements removeAnsiNullStatements)
        {
            _ensureStatementsHaveIds = ensureStatementsHaveIds;
            _generateDescriptions = generateDescriptions;
            _removeAnsiNullStatements = removeAnsiNullStatements;
        }


        public void Migrate(Database database)
        {
            _ensureStatementsHaveIds.Migrate(database);
            _generateDescriptions.Migrate(database);
            _removeAnsiNullStatements.Migrate(database);
        }
    }
}
