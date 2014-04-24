using Autofac;

namespace DBTM.Application
{
    public class AutofacRequestContainerProvider 
    {
        private static AutofacRequestContainerProvider _instance;

        private AutofacRequestContainerProvider()
        {
        }

        public static AutofacRequestContainerProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AutofacRequestContainerProvider();
                }
                return _instance;
            }
        }

        public ILifetimeScope RequestContainer { get; set; }
    }
}