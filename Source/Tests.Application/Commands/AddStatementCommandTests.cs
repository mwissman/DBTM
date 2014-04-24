using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.Views;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class AddStatementCommandTests
    {
        private AddStatementCommand _command;
        private IMainWindowView _view;
        private DatabaseVersion _databaseVersion;
        private SqlStatementCollection _sqlStatementCollection;


        [SetUp]
        public void Setup()
        {
            _view = MockRepository.GenerateMock<IMainWindowView>();

            _databaseVersion = MockRepository.GenerateMock<DatabaseVersion>();
            _sqlStatementCollection = MockRepository.GenerateMock<SqlStatementCollection>();
           
            _command = new AddStatementCommand(_view);
        }

        [TearDown]
        public void TearDown()
        {
            _view.VerifyAllExpectations();
            _databaseVersion.VerifyAllExpectations();
            _sqlStatementCollection.VerifyAllExpectations();
        }

        [Test]
        public void CreatesANewStatementAndSelectsItWhenValidVersionGiven()
        {
            _databaseVersion.Stub(dv => dv.PreDeploymentStatements).Return(_sqlStatementCollection);
            _sqlStatementCollection.Expect(c => c.Add(Arg<SqlStatement>.Matches(s=>s.IsEditable==true && s.Description=="" && s.RollbackSQL=="" && s.UpgradeSQL=="")));
            _sqlStatementCollection.Expect(c => c.SetCanMoveUpDownOnAllStatements());
            _view.Expect(v => v.SelectedSqlStatementType).Return(SqlStatementType.PreDeployment);
            _view.Expect(v => v.UpdateSelectedStatement(Arg<SqlStatement>.Matches(s => s.IsEditable == true && s.Description == "" && s.RollbackSQL == "" && s.UpgradeSQL == "")));

            _command.Execute(_databaseVersion);
        }

        [Test]
        public void CreatesANewStatementAndSelectsItWhenValidVersionGivenForBackfill()
        {
            _databaseVersion.Stub(dv => dv.BackfillStatements).Return(_sqlStatementCollection);
            _sqlStatementCollection.Expect(c => c.Add(Arg<SqlStatement>.Matches(s => s.IsEditable == true && s.Description == "" && s.RollbackSQL == "" && s.UpgradeSQL == "")));
            _sqlStatementCollection.Expect(c => c.SetCanMoveUpDownOnAllStatements());
            _view.Expect(v => v.SelectedSqlStatementType).Return(SqlStatementType.Backfill);
            _view.Expect(v => v.UpdateSelectedStatement(Arg<SqlStatement>.Matches(s => s.IsEditable == true && s.Description == "" && s.RollbackSQL == "" && s.UpgradeSQL == "")));

            _command.Execute(_databaseVersion);
        }

        [Test]
        public void CreatesANewStatementAndSelectsItWhenValidVersionGivenForPostDeployment()
        {
            _databaseVersion.Stub(dv => dv.PostDeploymentStatements).Return(_sqlStatementCollection);
            _sqlStatementCollection.Expect(c => c.Add(Arg<SqlStatement>.Matches(s => s.IsEditable == true && s.Description == "" && s.RollbackSQL == "" && s.UpgradeSQL == "")));
            _sqlStatementCollection.Expect(c => c.SetCanMoveUpDownOnAllStatements());
            _view.Expect(v => v.SelectedSqlStatementType).Return(SqlStatementType.PostDeployment);
            _view.Expect(v => v.UpdateSelectedStatement(Arg<SqlStatement>.Matches(s => s.IsEditable == true && s.Description == "" && s.RollbackSQL == "" && s.UpgradeSQL == "")));

            _command.Execute(_databaseVersion);
        }

        [Test]
        public void DoesNothingIfInvalidVersion()
        {
            _command.Execute(null);

            _view.AssertWasNotCalled(v => v.UpdateSelectedStatement(Arg<SqlStatement>.Is.Anything));
        }


        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}