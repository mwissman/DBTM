using System;
using DBTM.Cmd;
using DBTM.Cmd.Arguments;
using NUnit.Framework;

namespace Tests.Cmd.ArgumentsFactoryTests
{
   
    public partial class ArgumentsFactoryTests
    {
        [TestFixture]
        public class WhenCreatingAnArgumentsObject
        {
            private string[] _arguments;
            private ArgumentsFactory _factory;

            [SetUp]
            public void Setup()
            {
                _arguments = new string[] { "-blah=value","" };

                _factory = new ArgumentsFactory();
            }

            [TestCase("-command=fullbuild", typeof(FullBuildArguments))]
            [TestCase("", typeof(FullBuildArguments))]
            [TestCase("-command=createversion", typeof(CreateVersionArguments))]
            [TestCase("-command=compilescripts", typeof(CompileScriptsArguments))]
            public void ProperArguementsClassIsReturnBasedOnCommandLine(string commandString, Type expectedType)
            {
                _arguments[1] = commandString;
                IArguments result = _factory.Create(_arguments);

                Assert.That(result, Is.TypeOf(expectedType));
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void WhenCommandIsUnknownAnExceptionIsThrow()
            {
                _arguments[1] = "-command=unknown";
                IArguments result = _factory.Create(_arguments);

            }
        }
    }
}