using System;
using System.ComponentModel;
using System.Linq.Expressions;
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
    public class SaveDatabaseCommandTests
    {
        private SaveDatabaseCommand _command;
        private IDatabaseSchemaViewModel _viewModel;
        private IMainWindowView _view;
        private IApplicationSettings _applicationSettings;
        private IDatabaseRepository _databaseRepository;

        [SetUp]
        public void Setup()
        {
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _applicationSettings = MockRepository.GenerateMock<IApplicationSettings>();
            _databaseRepository = MockRepository.GenerateMock<IDatabaseRepository>();

            _viewModel.Stub(vm => vm.Settings).Return(_applicationSettings);

            _command = new SaveDatabaseCommand(_view, _viewModel, _databaseRepository);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel.VerifyAllExpectations();
            _view.VerifyAllExpectations();
            _applicationSettings.VerifyAllExpectations();
            _databaseRepository.VerifyAllExpectations();
        }

        [Test]
        public void AsksUserForNewFileNameIfFileNameNotSetAndResultIsBlankCommandWillDoNothing()
        {
            _applicationSettings.Expect(s => s.DatabaseFilePath).Return(string.Empty);
            _view.Expect(v => v.AskUserForNewFilePath()).Return(string.Empty);

            _command.Execute(null);
        }

        [Test]
        public void AsksUserForNewFileNameIfFileNameNotSetAndUseNewFileName()
        {
            string databaseFilePath = "something";
            Database database = new Database("something");
            _applicationSettings.Expect(s => s.DatabaseFilePath).Return(string.Empty);
            _view.Expect(v => v.AskUserForNewFilePath()).Return(databaseFilePath);
            _viewModel.Expect(vm => vm.Database).Return(database);
            _databaseRepository.Expect(r => r.Save(database, databaseFilePath));

            _command.Execute(null);
        }

        [Test]
        public void FileIsSet()
        {
            string databaseFilePath = "something";
            Database database = new Database("something");

            _viewModel.Expect(vm => vm.Database).Return(database);
            _applicationSettings.Expect(s => s.DatabaseFilePath).Return(databaseFilePath);
            _databaseRepository.Expect(r => r.Save(database, databaseFilePath));

            _command.Execute(null);
        }

        [Test]
        public void CanExecuteReturnsTrueIfDatabaseIsEditable()
        {
            _viewModel.Stub(vm => vm.Database).Return(new Database("test"));

            Assert.IsTrue(_command.CanExecute(null));
        }

        [Test]
        public void CanExecuteReturnsFalseIfDatabaseIsNotEditable()
        {
            _viewModel.Stub(vm => vm.Database).Return(new EmptyDatabase());

            Assert.IsFalse(_command.CanExecute(null));
        }

        [Test]
        public void CanExecuteChangedFiresWhenSettingANewDatabaseOnTheModelView()
        {
            var propertyName = ((Expression<Func<DatabaseSchemaViewModel, object>>)(vm => vm.Database)).GetMemberName();

            bool canExecuteChangedFired = false;

            _command.CanExecuteChanged += (o, args) => canExecuteChangedFired = true;

            _viewModel.Raise(vm => vm.PropertyChanged += null, _viewModel, new PropertyChangedEventArgs(propertyName));

            Assert.IsTrue(canExecuteChangedFired);
        }
    }
}