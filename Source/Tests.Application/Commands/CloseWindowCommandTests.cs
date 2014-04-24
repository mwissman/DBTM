using System;
using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class CloseWindowCommandTests
    {
        private CloseWindowCommand _command;
        private IMainWindowView _view;
       
        private IDatabaseSchemaViewModel _viewModel;
        private Database _database;


        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<IMainWindowView>();
            
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();

            _database = MockRepository.GenerateMock<Database>("name");

            _command = new CloseWindowCommand(_view, _viewModel);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _database.VerifyAllExpectations();
            _viewModel.VerifyAllExpectations();
        }

        [Test]
        public void IfDatabaseIsDirtyPromptsUserToExitDoesNothingIfUserDoesNotWantToAbandonChanges()
        {
            _viewModel.Stub(vm => vm.Database).Return(_database);

            _database.Expect(d => d.IsSaved).Return(false);
            
            _view.Expect(v => v.AskUserChangesShouldBeAbandoned()).Return(false);

            _command.Execute(null);
            Assert.IsFalse(_command.CanCloseWindow);
            
        }

        [Test]
        public void IfDatabaseIsDirtyPromptsUserToExitClosesWindowAndExitsIfUserWantsToAbandonChanges()
        {
            _viewModel.Stub(vm => vm.Database).Return(_database);
            _database.Expect(d => d.IsSaved).Return(false);
            _view.Expect(v => v.AskUserChangesShouldBeAbandoned()).Return(true);

            _command.Execute(null);
            Assert.IsTrue(_command.CanCloseWindow);
        }

        [Test]
        public void IfDatabaseIsNotDirtyDoesNotPromptUser()
        {
            _viewModel.Stub(vm => vm.Database).Return(_database);
            _database.Expect(d => d.IsSaved).Return(true);
            
            _command.Execute(null);
            Assert.IsTrue(_command.CanCloseWindow);
        }


        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}