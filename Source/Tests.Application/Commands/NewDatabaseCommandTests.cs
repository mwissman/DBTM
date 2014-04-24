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
    public class NewDatabaseCommandTests
    {
        private NewDatabaseCommand _command;
        private IDatabaseSchemaViewModel _viewModel;
        private IMainWindowView _view;
        private IApplicationSettings _applicationSettings;

        [SetUp]
        public void Setup()
        {
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _applicationSettings = MockRepository.GenerateMock<IApplicationSettings>();

            _command = new NewDatabaseCommand(_view,_viewModel);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel.VerifyAllExpectations();
            _view.VerifyAllExpectations();
            _applicationSettings.VerifyAllExpectations();
        }

        [Test]
        public void GetsNameFromViewSetsNewDatabaseOnViewModel()
        {
            string databaseName = "New Database";
            _view.Expect(v => v.AskUserForNewDatabasename()).Return(databaseName);
            _viewModel.Stub(vm => vm.Settings).Return(_applicationSettings);
            _viewModel.Expect(vm => vm.Database = Arg<Database>.Matches(d => d.DbName == databaseName));
            _applicationSettings.Expect(s => s.DatabaseFilePath = string.Empty);

            _command.Execute(null);
        }

        [Test]
        public void GetsNameFromViewAndDoesNothingIfNameIsBlankSetsNewDatabaseOnViewModel()
        {
            _view.Expect(v => v.AskUserForNewDatabasename()).Return(string.Empty);

            _command.Execute(null);
        }


        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}