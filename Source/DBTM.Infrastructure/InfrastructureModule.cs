using Autofac;
using Autofac.Builder;

namespace DBTM.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new StreamWriter())
                .As<IStreamWriter>()
                .InstancePerDependency();

            builder.Register(c => new StreamReader())
               .As<IStreamReader>()
               .InstancePerDependency();
        }
    }
}