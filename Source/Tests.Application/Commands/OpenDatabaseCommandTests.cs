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
    public class OpenDatabaseCommandTests
    {
        private OpenDatabaseCommand _command;
        private ICanOpenDatabasesView _view;
        private IDatabaseSchemaViewModel _viewModel;
        private IDatabaseRepository _databaseRepository;
        
        private IApplicationSettings _applicationSettings;
        private Database _database;

        [SetUp]
        public void SetUp()
        {
            _view = MockRepository.GenerateMock<ICanOpenDatabasesView>();
            _viewModel = MockRepository.GenerateMock<IDatabaseSchemaViewModel>();
            _databaseRepository = MockRepository.GenerateMock<IDatabaseRepository>();
            _applicationSettings = MockRepository.GenerateMock<IApplicationSettings>();
            _database = MockRepository.GenerateMock<Database>("");

            _command = new OpenDatabaseCommand(_view,_viewModel,_databaseRepository);
        }


        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _viewModel.VerifyAllExpectations();
            _databaseRepository.VerifyAllExpectations();
            _database.VerifyAllExpectations();
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            var actual = _command.CanExecute(null);
            Assert.IsTrue(actual);
        }


        [Test]
        public void ExecutePromptsForFileAndLoadDatabaseFromFileWhenAFileIsSeleted()
        {
            string dbschema = "c:\\somewhere.dbschema";
            _view.Expect(v => v.OpenFile()).Return(dbschema);

            _databaseRepository.Expect(dr => dr.Load(dbschema)).Return(_database);
            _viewModel.Expect(mv => mv.Settings).Return(_applicationSettings);
            _applicationSettings.Expect(s => s.DatabaseFilePath = dbschema);
            _viewModel.Expect(vm => vm.Database = _database);
            _viewModel.Stub(vm => vm.Database).Return(_database);
            _database.Expect(d => d.IsSaved).Return(true);
           
            _command.Execute(null);
        }

        [Test]
        public void ExecuteLoadDatabaseFromFileWhenAFileIsPassedIn()
        {
            
            string schemaFile = "c:\\somewhere.dbschema";

            _databaseRepository.Expect(dr => dr.Load(schemaFile)).Return(_database);
            _viewModel.Expect(mv => mv.Settings).Return(_applicationSettings);
            _applicationSettings.Expect(s => s.DatabaseFilePath = schemaFile);
            
            _database.Expect(d => d.IsSaved).Return(true);
            _viewModel.Expect(vm => vm.Database = _database);
            _viewModel.Stub(vm => vm.Database).Return(_database);

            _command.Execute(schemaFile);
            _view.AssertWasNotCalled(v => v.OpenFile());
        }

        [Test]
        public void ExecutePromptsToSeeCloseIfDatabaseIsDirtyDoesNotLoadDatabaseIfShouldCloseIsFalse()
        {
            string dbschema = "c:\\somewhere.dbschema";
            _view.Expect(v => v.OpenFile()).Return(dbschema);

            _viewModel.Stub(mv => mv.Settings).Return(_applicationSettings);
            _viewModel.Stub(vm => vm.Database).Return(_database);
            _database.Expect(d => d.IsSaved).Return(false);

            _view.Expect(v => v.AskUserChangesShouldBeAbandoned()).Return(false);

            _command.Execute(null);
            _databaseRepository.AssertWasNotCalled(dr => dr.Load(Arg<string>.Is.Anything));
        }

    }
}
