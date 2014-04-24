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
    public class SetConnectionStringCommandTests
    {
        private SetConnectionStringCommand _command;
        private IDatabaseSchemaViewModel _viewModel;
        private IMainWindowView _view;
        private IApplicationSettings _applicationSettings;
        private ITestSqlServerConnectionStrings _connectionStringTester;
        private string _oldConnectionString;

        [SetUp]
        public void Setup()
        {
            _connectionStringTester = MockRepository.GenerateMock<ITestSqlServerConnectionStrings>();
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _applicationSettings = MockRepository.GenerateStub<IApplicationSettings>();
            _viewModel.Stub(vm => vm.Settings).Return(_applicationSettings);

            _oldConnectionString = "Old connection string";
            _viewModel.Settings.ConnectionString = _oldConnectionString;

            _command = new SetConnectionStringCommand(_view, _connectionStringTester, _viewModel);
        }

        [TearDown]
        public void TearDown()
        {
            _connectionStringTester.VerifyAllExpectations();
            _view.VerifyAllExpectations();

            _viewModel.VerifyAllExpectations();
            _applicationSettings.VerifyAllExpectations();
        }

        [Test]
        public void AsksForConnectionFromUserTestsItIfValidGivesToViewModelWhenValid()
        {
            string validConnectionString = "valid connection string";
            SetConnectionStringResult result = new SetConnectionStringResult(false, validConnectionString);

            _view.Expect(v => v.AskUserForConnectionString()).Return(result);
            _connectionStringTester.Expect(t => t.IsValid(validConnectionString)).Return(true);
           
            _command.Execute(null);

            Assert.AreEqual(validConnectionString,_viewModel.Settings.ConnectionString);
        }

        [Test]
        public void AsksForConnectionFromUserCancelsConnectionStringNotSetOrTested()
        {
            string connectionString = "what ever";
            SetConnectionStringResult result = new SetConnectionStringResult(true, connectionString);

            _view.Expect(v => v.AskUserForConnectionString()).Return(result);
           
            _command.Execute(null);

            Assert.AreNotEqual(connectionString, _viewModel.Settings.ConnectionString);

            _connectionStringTester.AssertWasNotCalled(t => t.IsValid(Arg<string>.Is.Anything));
        }

        [Test]
        public void AsksForConnectionFromUserTestsItIfValidThrowsAwayConnectionStringIfNotValid()
        {
            string validConnectionString = "invalidvalid connection string";
            SetConnectionStringResult result = new SetConnectionStringResult(false, validConnectionString);

            _view.Expect(v => v.AskUserForConnectionString()).Return(result);
            _connectionStringTester.Expect(t => t.IsValid(validConnectionString)).Return(false);
            _view.DisplayError("Invalid Connection String!");

            _command.Execute(null);

            Assert.AreEqual(_oldConnectionString, _viewModel.Settings.ConnectionString);
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}