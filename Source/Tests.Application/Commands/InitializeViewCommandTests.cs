using System;
using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Application.Views;
using DBTM.Domain;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class InitializeViewCommandTests
    {
        private IMainWindowView _view;
        private IArgumentsProvider _argumentsProvider;
        private OpenDatabaseCommand _openDatabaseCommand;
        private InitializeViewCommand _command;

        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _argumentsProvider = MockRepository.GenerateMock<IArgumentsProvider>();
            _openDatabaseCommand = MockRepository.GenerateMock<OpenDatabaseCommand>((ICanOpenDatabasesView) null,
                                                                                    (IDatabaseSchemaViewModel) null,
                                                                                    (IDatabaseRepository) null);

            _command = new InitializeViewCommand(_view,_argumentsProvider, _openDatabaseCommand );
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _argumentsProvider.VerifyAllExpectations();
            _openDatabaseCommand.VerifyAllExpectations();
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }

        [Test]
        public void CanExecuteCallsOpenDatabaseIfFileInArguments()
        {
             string file="somewhere";
            _argumentsProvider.Expect(ap => ap.HasFile).Return(true);
            _argumentsProvider.Expect(ap => ap.FilePath).Return(file);
            _openDatabaseCommand.Expect(c => c.CanExecute(null)).Return(true);
            _openDatabaseCommand.Expect(c => c.Execute(file));
            _view.Expect(v => v.Show());
           
            _command.Execute(null);
        }

        [Test]
        public void CanExecuteShowsView()
        {
            string file = "somewhere";
            _argumentsProvider.Expect(ap => ap.HasFile).Return(false);

            _view.Expect(v => v.Show());

            _command.Execute(null);
        }

    }
}