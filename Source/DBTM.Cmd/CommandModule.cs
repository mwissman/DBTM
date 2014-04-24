using System;
using Autofac;
using DBTM.Application;
using DBTM.Application.SQL;
using DBTM.Application.Views;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using DBTM.Cmd.Views;
using DBTM.Domain;
using DBTM.Infrastructure;
using DBTM.Application.Commands;

namespace DBTM.Cmd
{
    public class CommandModule : Module
    {
        private readonly string[] _args;
        public const string ARGUMENTS_PARAMETER = "Arguments";

        public CommandModule(string[] args)
        {
            _args = args;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new ApplicationCommandModule());

            builder.Register(c => new ArgumentsFactory().Create(_args))
                .As<IArguments>()
                .SingleInstance();


            builder.Register(c => c.Resolve<IArguments>() as ICompileScriptsArguments).As<ICompileScriptsArguments>();

            builder.Register(c => c.Resolve<IArguments>() as ICreateVersionArguments).As<ICreateVersionArguments>();

            builder.Register(c => c.Resolve<IArguments>() as IFullBuildArguments).As<IFullBuildArguments>();

            builder.Register(c => c.Resolve<IArguments>() as IRunDirectoryOfSqlArguments).As<IRunDirectoryOfSqlArguments>();

       

            builder.Register<Func<string[], IArguments>>(c=>a=>new ArgumentsFactory().Create(a));

            builder.Register(c => new FullBuildApplicationRunner(c.Resolve<IDatabaseRepository>(),
                                                                 c.Resolve<IDatabaseBuildService>(),
                                                                 c.Resolve<ISqlServerDatabaseSettingsBuilder>()))
                .As<IApplicationRunner<IFullBuildArguments>>()
                .SingleInstance();

            builder.Register(c => new CompileScriptsApplicationRunner(c.Resolve<IDatabaseRepository>(), c.Resolve<CompileVersionCommand>()))
                .As<IApplicationRunner<ICompileScriptsArguments>>()
                .SingleInstance();

            builder.Register(
                c =>
                new RunDirectoryOfSqlRunner(c.Resolve<ISqlRunner>(),
                    c.Resolve<ISqlServerDatabaseSettingsBuilder>(),
                    c.Resolve<ISqlFileReader>()
                    ))
                .As<IApplicationRunner<IRunDirectoryOfSqlArguments>>()
                .SingleInstance();


            builder.Register(c => new CreateVersionApplicationRunner(c.Resolve<IDatabaseRepository>(), c.Resolve<IMigrator>()))
                .As<IApplicationRunner<ICreateVersionArguments>>()
                .SingleInstance();

            builder.Register(c => new SqlServerDatabaseSettingsBuilder())
                .As<ISqlServerDatabaseSettingsBuilder>()
                .SingleInstance();

            builder.Register(c => new CommandLineCompileVersionView(c.Resolve<ICompileScriptsArguments>())).
                As<ICompileVersionView>().
                InstancePerLifetimeScope();
        }
    }
}