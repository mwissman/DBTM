using System;
using System.Windows.Input;
using Autofac;
using DBTM.Application.ViewModels;
using DBTM.Application.Factories;
using DBTM.Application.SQL;
using DBTM.Infrastructure;

namespace DBTM.Application
{

    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new SQLConnectionFactory()).
                As<IDatabaseConnectionFactory>().
                SingleInstance();

            builder.Register(c => new SqlRunner(c.Resolve<IDbCommandFactory>(),
                                                c.Resolve<IDatabaseConnectionFactory>())).
                As<ISqlRunner>().
                InstancePerLifetimeScope();

            builder.Register(c => new DatabaseBuildService(c.Resolve<ISqlRunner>(),
                                                           c.Resolve<ISqlServerDatabase>(),
                                                           c.Resolve<ISqlScriptRepository>()))
                   .As<IDatabaseBuildService>()
                   .InstancePerLifetimeScope();

            builder.Register(c => new SqlServerDatabase(c.Resolve<ISqlRunner>(),
                                                        c.Resolve<ISqlScriptRepository>()))
                .As<ISqlServerDatabase>()
                .InstancePerLifetimeScope();

            builder.Register(c => new SqlScriptRepository(c.Resolve<IStreamReader>()))
                .As<ISqlScriptRepository>()
                .InstancePerLifetimeScope();

            builder.Register(c => new DbCommandFactory()).
                As<IDbCommandFactory>().
                InstancePerLifetimeScope();

            builder.Register(c => new SqlFileWriter(c.Resolve<IStreamWriter>()))
                .As<ISqlFileWriter>()
                .InstancePerLifetimeScope();

            builder.Register(c => new SqlFileReader())
                .As<ISqlFileReader>()
                .InstancePerLifetimeScope();

            builder.Register(c => new SqlServerConnectionStringTester())
                .As<ITestSqlServerConnectionStrings>()
                .InstancePerLifetimeScope();

            //Model Views
            builder.Register(c => new DatabaseSchemaViewModel(c.Resolve<Func<Type,ICommand>>())).
                As<IDatabaseSchemaViewModel>().
                InstancePerLifetimeScope();
            
        }
    }
}
