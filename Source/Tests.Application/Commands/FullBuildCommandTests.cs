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
    public class FullBuildCommandTests
    {
        private FullBuildCommand _command;
        private IDatabaseSchemaViewModel _viewModel;
        private IMainWindowView _view;
       
        private IDatabaseBuildService _databaseBuildService;

        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _databaseBuildService = MockRepository.GenerateMock<IDatabaseBuildService>();
            
            _command = new FullBuildCommand(_view,_viewModel,_databaseBuildService);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _viewModel.VerifyAllExpectations();
            _databaseBuildService.VerifyAllExpectations();
        }

        [Test]
        public void DelegatesToDatabaseBuildServiceToBuildTheEntireDatabaseAndDisplaysSuccessMessage()
        {
            string databaseFilePath = "some path";
            string databaseName = "DatabaseName";
            string server = "server";
            string username = "username";

            FullBuildDialogResults settings = new FullBuildDialogResults();
            settings.DatabaseFilePath = databaseFilePath;
            settings.DatabaseName = databaseName;
            settings.DatabaseServer = server;
            settings.DatabaseUsername = username;
            settings.WasCanceled = false;

            
            var database = new Database(databaseName);

            _viewModel.Expect(v => v.Database).Return(database);
            _view.Expect(v => v.AskUserForFullBuildParameters(databaseName)).Return(settings);

            string buildMessage = "message";

            DatabaseBuildResult result = new DatabaseBuildResult(true, buildMessage);

            _databaseBuildService.Expect(db => db.FullBuild(Arg<Database>.Is.Equal(database),
                                                            Arg<ISqlServerDatabaseSettings>.
                                                                Matches(s =>
                                                                        s.DatabaseFilePath == databaseFilePath &&
                                                                        s.DatabaseName == databaseName &&
                                                                        s.Server == server))).Return(result);

            _view.Expect(v => v.ShowBuildResultMessage(buildMessage,databaseName));

            _command.Execute(null);
        }

        [Test]
        public void DoesNotBuildIfUserCancelsDialog()
        {
            string databaseName="database name";
            var database = new Database(databaseName);

            FullBuildDialogResults settings = new FullBuildDialogResults();

            settings.WasCanceled = true;

            _viewModel.Expect(v => v.Database).Return(database);
            _view.Expect(v => v.AskUserForFullBuildParameters(databaseName)).Return(settings);

            _command.Execute(null);

            _databaseBuildService.AssertWasNotCalled(db => db.FullBuild(Arg<Database>.Is.Anything, Arg<ISqlServerDatabaseSettings>.Is.Anything));
            _view.AssertWasNotCalled(v => v.ShowBuildResultMessage(Arg<string>.Is.Anything,Arg<string>.Is.Anything));
        }

        [Test]
        public void BuildFullDatabaseCanrepopulateTheDialogBox()
        {
            string databaseFilePath = "some path";
            string databaseName = "DatabaseName";
            string server = "server";
            string username = "username";

            FullBuildDialogResults settings = new FullBuildDialogResults();
            settings.DatabaseFilePath = databaseFilePath;
            settings.DatabaseName = databaseName;
            settings.DatabaseServer = server;
            settings.DatabaseUsername = username;
            settings.WasCanceled = false;

            var database = new Database(databaseName);

            _viewModel.Expect(v => v.Database).Return(database).Repeat.AtLeastOnce();
            _view.Expect(v => v.AskUserForFullBuildParameters(databaseName)).Return(settings).Repeat.AtLeastOnce();
            _view.Expect(v => v.AskUserForFullBuildParameters(databaseName,settings)).Return(settings).Repeat.AtLeastOnce();


            string buildMessage = "message";

            DatabaseBuildResult result = new DatabaseBuildResult(true, buildMessage);

            _databaseBuildService.Expect(db => db.FullBuild(Arg<Database>.Is.Equal(database),
                                                            Arg<ISqlServerDatabaseSettings>.
                                                                Matches(s =>
                                                                        s.DatabaseFilePath == databaseFilePath &&
                                                                        s.DatabaseName == databaseName &&
                                                                        s.Server == server ))).Return(result).Repeat.Twice();

            _view.Expect(v => v.ShowBuildResultMessage(buildMessage, databaseName)).Repeat.Twice();

            _command.Execute(null);
            _command.Execute(null);
        }

        [Test]
        public void FullBuildDisplaysErrorIfCannotFullBuildTheDatabase()
        {
            Database database = new EmptyDatabase();
            _viewModel.Expect(v => v.Database).Return(database);

            _view.Expect(v => v.DisplayError("Cannot run Full Build on an empty database. Please set a database name."));

            _command.Execute(null);

            _view.AssertWasNotCalled(v => v.AskUserForFullBuildParameters(Arg<string>.Is.Anything));
            _databaseBuildService.AssertWasNotCalled(db => db.FullBuild(Arg<Database>.Is.Anything, Arg<ISqlServerDatabaseSettings>.Is.Anything));
            _view.AssertWasNotCalled(v => v.ShowBuildResultMessage(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }


        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}