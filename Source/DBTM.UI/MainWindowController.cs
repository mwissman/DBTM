using System;
using Autofac;
using DBTM.Application;
using DBTM.Application.Views;

namespace DBTM.UI
{
    public class MainWindowController
    {
        private readonly ILifetimeScope _applicationContainer;
        private ILifetimeScope _requestContainer;
        protected Func<ILifetimeScope, IMainWindowView> ResolveMainWindowViewFunc = c => c.Resolve<IMainWindowView>();

        public MainWindowController(ILifetimeScope applicationContainer)
        {
            _applicationContainer = applicationContainer;
        }

        public void Start()
        {
            _requestContainer = _applicationContainer.BeginLifetimeScope();
            AutofacRequestContainerProvider.Instance.RequestContainer = _requestContainer;
            var view = ResolveMainWindowViewFunc(_requestContainer);
           
            view.Initialize();
        }

        public void Stop()
        {
            AutofacRequestContainerProvider.Instance.RequestContainer = null;
            _requestContainer.Dispose();
        }
    }
}