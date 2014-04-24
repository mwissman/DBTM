using System;
using DBTM.Application.Views;
using DBTM.Cmd.Arguments;
using DBTM.Cmd.Views;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd
{
    [TestFixture]
    public class CompileScriptsViewTests
    {
        private ICompileVersionView _view;
        private ICompileScriptsArguments _arguments;

        [SetUp]
        public void Setup()
        {
            _arguments = MockRepository.GenerateMock<ICompileScriptsArguments>();

            _view = new CommandLineCompileVersionView(_arguments);
        }

        [TearDown]
        public void TearDown()
        {
            _arguments.VerifyAllExpectations();
        }

        [Test]
        public void AskUserForCompiledSqlFolderPathGetsPathFromArguements()
        {
            string scriptsDirectory = "c:\\somewhere\\scripts";

            _arguments.Stub(a => a.CompiledScriptDirectory).Return(scriptsDirectory);

            var actual = _view.AskUserForCompiledSqlFolderPath();

            Assert.AreEqual(scriptsDirectory,actual);
        }

        [Test]
        public void AskUserForCrossDatabasePrefixReturnsEmptyString()
        {
            var result = _view.AskUserForCrossDatabasePrefix();

            Assert.AreEqual(result, String.Empty);
        }

        [Test]
        public void DisplayStatusMessageDoesNotBlowUp()
        {
            _view.DisplayStatusMessage("message");
        }
    }
}