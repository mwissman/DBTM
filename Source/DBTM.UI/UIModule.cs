using System;
using Autofac;
using Autofac.Builder;
using DBTM.Application;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Infrastructure;
using Microsoft.Win32;
using DBTM.Domain;

namespace DBTM.UI
{
    public class UIModule : Module
    {
        private readonly string[] _arguments;

        public UIModule(string[] arguments)
        {
            _arguments = arguments;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new ApplicationCommandModule());

            builder.Register(c => new OpenFileDialog()).
                As<OpenFileDialog>().
                InstancePerLifetimeScope();

            builder.Register(c => new SaveFileDialog()).
                As<SaveFileDialog>().
                InstancePerLifetimeScope();

            builder.Register(c => new MainWindowController(c.Resolve<ILifetimeScope>())).
                As<MainWindowController>().
                SingleInstance();

            builder.Register(c => new MainWindow(c.Resolve<OpenFileDialog>(),
                                                 c.Resolve<SaveFileDialog>(),
                                                 c.Resolve<IDatabaseSchemaViewModel>())).
                As<IMainWindowView>().
                As<ICanOpenDatabasesView>().
                As<ICompileVersionView>().
                InstancePerLifetimeScope();

            builder.Register(c => new ArgumentsProvider(_arguments)).
                As<IArgumentsProvider>().
                SingleInstance();
        }
    }
}