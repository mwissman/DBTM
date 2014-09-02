using System;
using System.Windows.Input;
using Autofac;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;

namespace DBTM.Application
{
    public class ApplicationCommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new Compiler(c.Resolve<ISqlFileWriter>(),
                                               c.Resolve<ISqlScriptRepository>()))
                   .As<ICompiler>()
                   .InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var componentContext = c.Resolve<IComponentContext>();

                    return (Func<Type, ICommand>) (t => componentContext.Resolve(t) as ICommand);
                }
                )
                   .As<Func<Type, ICommand>>()
                   .InstancePerLifetimeScope();

            builder.Register(c => new OpenDatabaseCommand(c.Resolve<ICanOpenDatabasesView>(),
                                                          c.Resolve<IDatabaseSchemaViewModel>(),
                                                          c.Resolve<IDatabaseRepository>())).
                    As<OpenDatabaseCommand>()
                   .InstancePerLifetimeScope();

            builder.Register(c => new InitializeViewCommand(c.Resolve<IMainWindowView>(),
                                                            c.Resolve<IArgumentsProvider>(),
                                                            c.Resolve<OpenDatabaseCommand>())).
                    As<InitializeViewCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new NewDatabaseCommand(c.Resolve<IMainWindowView>(),
                                                         c.Resolve<IDatabaseSchemaViewModel>())).
                    As<NewDatabaseCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new SaveDatabaseAsCommand(c.Resolve<IMainWindowView>(),
                                                            c.Resolve<IDatabaseSchemaViewModel>(),
                                                            c.Resolve<IDatabaseRepository>())).
                    As<SaveDatabaseAsCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new SaveDatabaseCommand(c.Resolve<IMainWindowView>(),
                                                          c.Resolve<IDatabaseSchemaViewModel>(),
                                                          c.Resolve<IDatabaseRepository>())).
                    As<SaveDatabaseCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new MoveStatementUpCommand()).
                    As<MoveStatementUpCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new MoveStatementDownCommand()).
                    As<MoveStatementDownCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new RemoveStatementCommand()).
                    As<RemoveStatementCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new SetConnectionStringCommand(c.Resolve<IMainWindowView>(),
                                                                 c.Resolve<ITestSqlServerConnectionStrings>(),
                                                                 c.Resolve<IDatabaseSchemaViewModel>())).
                    As<SetConnectionStringCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new FullBuildCommand(c.Resolve<IMainWindowView>(),
                                                       c.Resolve<IDatabaseSchemaViewModel>(),
                                                       c.Resolve<IDatabaseBuildService>())).
                    As<FullBuildCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new AboutCommand(c.Resolve<IMainWindowView>())).
                    As<AboutCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new AddVersionCommand(c.Resolve<IMainWindowView>(),
                                                        c.Resolve<IDatabaseSchemaViewModel>(),
                                                        c.Resolve<IMigrator>())).
                    As<AddVersionCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new AddStatementCommand(c.Resolve<IMainWindowView>())).
                    As<AddStatementCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new CompileVersionCommand(c.Resolve<ICompileVersionView>(),
                                                            c.Resolve<ICompiler>())).
                    As<CompileVersionCommand>().
                    InstancePerLifetimeScope();

            builder.Register(c => new CompileAllVersionsCommand(c.Resolve<ICompileVersionView>(),
                                                                c.Resolve<IDatabaseSchemaViewModel>(),
                                                                c.Resolve<ICompiler>()))
                   .As<CompileAllVersionsCommand>()
                   .InstancePerLifetimeScope();

            builder.Register(c => new CloseWindowCommand(c.Resolve<IMainWindowView>(),
                                                         c.Resolve<IDatabaseSchemaViewModel>())).
                    As<CloseWindowCommand>().
                    InstancePerLifetimeScope();
        }
    }
}