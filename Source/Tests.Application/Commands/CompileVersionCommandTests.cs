using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.Views;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class CompileVersionCommandTests
    {
        private CompileVersionCommand _command;
        private ICompileVersionView _view;
        private ICompiler _compiler;
        
        private Database _database;

        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<ICompileVersionView>();
            _compiler = MockRepository.GenerateMock<ICompiler>();
            _database = MockRepository.GenerateMock<Database>("test"); 
            
            _command = new CompileVersionCommand(_view, _compiler);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _compiler.VerifyAllExpectations();
            _database.VerifyAllExpectations();   
        }

        [TestCase("")]
        [TestCase(null)]
        public void PromptsForFolderAndDoesNothingIfUserCancelsFolderDialogBox(string folderPath)
        {
            _view.Expect(v => v.AskUserForCompiledSqlFolderPath()).Return(folderPath);

            _command.Execute(_database);

            _compiler.AssertWasNotCalled(sfw => sfw.CompileLatestVersion(Arg<Database>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [TestCase(null)]
        [TestCase("blah")]
        public void DoesNothingIfParameterIsNotDatabase(object commandParameter)
        {
            _command.Execute(commandParameter);

            _view.AssertWasNotCalled(v => v.AskUserForCompiledSqlFolderPath());
            _compiler.AssertWasNotCalled(sfw => sfw.CompileLatestVersion(Arg<Database>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }
       
        [Test]
        public void PromptsForPreAndPostDeploymentScriptsWhenStatementsExistForAll()
        {
            string compiledSqlFolderPath = "c:\\folder\\path";
            string databasePrefix = "DatabasePrefix_";
          
            _view.Expect(v => v.AskUserForCompiledSqlFolderPath()).Return(compiledSqlFolderPath);
            _view.Expect(v => v.AskUserForCrossDatabasePrefix()).Return(databasePrefix);

            _command.Execute(_database);

            _compiler.AssertWasCalled(c => c.CompileLatestVersion(_database, compiledSqlFolderPath, databasePrefix));
            _view.AssertWasCalled(v => v.DisplayStatusMessage("Compile Sql Version completed successfully"));
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}