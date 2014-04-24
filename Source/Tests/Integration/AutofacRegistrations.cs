using System;
using Autofac;
using Autofac.Core.Registration;
using DBTM.Application.ViewModels;
using DBTM.Cmd;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Runners;
using DBTM.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Integration
{
    [TestFixture]
    public class AutofacRegistrations
    {
        [TestCase(typeof(IApplicationRunner<ICompileScriptsArguments>),new[]{typeof(ICompileScriptsArguments)})]
        [TestCase(typeof(IApplicationRunner<IFullBuildArguments>),null)]
        [TestCase(typeof(IApplicationRunner<ICreateVersionArguments>),null)]
        public void AreAllCommandLineInterfaceClassesDownstreamClassesAutofacedFor(Type topLevelClass, Type[] runtimeRegisteredTypes)
        {
            AreClassesRegisteredFor(runtimeRegisteredTypes, topLevelClass, new CommandModule(new string[]{}));
        }

        [TestCase(typeof(MainWindowController), null)]
        [TestCase(typeof(IDatabaseSchemaViewModel), null)]
        public void AreAllGraphicalUserInterfaceClassesDownstreamClassesAutofacedFor(Type topLevelClass, Type[] runtimeRegisteredTypes)
        {
            AreClassesRegisteredFor(runtimeRegisteredTypes, topLevelClass, new UIModule(new string[]{}));
        }

        private void AreClassesRegisteredFor(Type[] runtimeRegisteredTypes, Type topLevelClass, Module module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);

            if (runtimeRegisteredTypes != null)
            {
                foreach (Type runtimeRegisteredType in runtimeRegisteredTypes)
                {
                    Type typeToUse = runtimeRegisteredType;

                    builder.Register(c => MockRepository.GenerateStub(typeToUse)).As(typeToUse);
                }
            }

            var topLevelContainer = builder.Build();

            try
            {
                var service = topLevelContainer.Resolve(topLevelClass);
                Assert.IsNotNull(service);
            }
            catch (ComponentNotRegisteredException ex)
            {
                Assert.Fail(String.Format("All downstream dependencies are resolved for type {0} failed to resolve. {1}", topLevelClass.Name, ex.Message), new object[] { ex });
            }
        }
    }
}
