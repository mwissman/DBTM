using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Autofac;
using Autofac.Builder;
using DBTM.Application;

namespace DBTM.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly ContainerBuilder _containerBuilder;
        private IContainer _applicationContainer;
        private MainWindowController _mainWindowController;

        public App()
        {
            _containerBuilder = new ContainerBuilder();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _containerBuilder.RegisterModule(new UIModule(e.Args));

            _applicationContainer = _containerBuilder.Build();

            _mainWindowController = _applicationContainer.Resolve<MainWindowController>();
            _mainWindowController.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mainWindowController.Stop();
            _applicationContainer.Dispose();
        }
    }
}
