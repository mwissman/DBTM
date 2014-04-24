using System;
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
    public class AddVersionCommandTests
    {
        private AddVersionCommand _command;
        private IDatabaseSchemaViewModel _viewModel;
        private IMainWindowView _view;
       
        private Database _database;
        private IMigrator _migrator;


        [SetUp]
        public void Setup()
        {
            
            _view = MockRepository.GenerateMock<IMainWindowView>();
            _viewModel = MockRepository.GenerateStub<IDatabaseSchemaViewModel>();
            _migrator = MockRepository.GenerateMock<IMigrator>();

            _database = MockRepository.GenerateMock<Database>();
            _viewModel.Database = _database;

            _command = new AddVersionCommand(_view, _viewModel, _migrator);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _viewModel.VerifyAllExpectations();
            _database.VerifyAllExpectations();
            _migrator.VerifyAllExpectations();
        }

        [Test]
        public void CreatesNewVersionSelectsNewVersionInView()
        {
            DatabaseVersion databaseVersion = new DatabaseVersion(1,DateTime.Now);

            _database.Expect(d => d.AddChangeset()).Return(databaseVersion);

            _view.Expect(v => v.UpdateSelectedVersion(databaseVersion));

            _command.Execute(null);

            _migrator.AssertWasCalled(m=>m.EnsureStatementsHaveIds(_database));
        }
 
        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}