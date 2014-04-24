using System;
using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class CompileAllVersionsTests
    {
        private CompileAllVersionsCommand _command;
        private ICompileVersionView _view;
      
        private IDatabaseSchemaViewModel _viewModel;
        private Database _database;
        private ICompiler _compiler;

        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<ICompileVersionView>();

            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _database = MockRepository.GenerateMock<Database>();
            _compiler = MockRepository.GenerateMock<ICompiler>();

            _command = new CompileAllVersionsCommand(_view, _viewModel, _compiler);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _compiler.VerifyAllExpectations();
            _viewModel.VerifyAllExpectations();
            _database.VerifyAllExpectations();
        }

        [Test]
        public void PromptsUserForFolderAndCompilesDatabaseAndSaves()
        {
            string compileScriptPath = "c:\\path\\somewhere";
          
            _view.Stub(v => v.AskUserForCompiledSqlFolderPath()).Return(compileScriptPath);
            _viewModel.Stub(vm => vm.Database).Return(_database);

            _command.Execute(null);

            _view.AssertWasCalled(v=>v.AskUserForCompiledSqlFolderPath());
            _view.AssertWasCalled(v=>v.DisplayStatusMessage("Compile Sql for all Versions completed successfully"));
            _compiler.AssertWasCalled(c => c.CompileAllVersions(_database,compileScriptPath));
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNothingIfUserCancelsFolderSelection(string compileScriptPath)
        {
            _view.Stub(v => v.AskUserForCompiledSqlFolderPath()).Return(compileScriptPath);

            _viewModel.Stub(vm => vm.Database).Return(_database);

            _command.Execute(null);

            _view.AssertWasCalled(v => v.AskUserForCompiledSqlFolderPath());
            _database.AssertWasNotCalled(d => d.CompileAllVersions(Arg<ICompiledSql>.Is.Anything));
            _compiler.AssertWasNotCalled(c=>c.CompileAllVersions(Arg<Database>.Is.Anything, Arg<string>.Is.Anything));
            _view.AssertWasCalled(v => v.DisplayStatusMessage("Compile Sql for all Versions was canceled"));
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}