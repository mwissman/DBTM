using Autofac;

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

            builder.Register(c => new Migrator(c.Resolve<IGuidFactory>()))
                .As<IMigrator>()
                .InstancePerLifetimeScope();
        }
    }
}
