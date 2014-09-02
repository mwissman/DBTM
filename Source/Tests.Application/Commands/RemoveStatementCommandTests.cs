using DBTM.Application.Commands;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class RemoveStatementCommandTests
    {
        private RemoveStatementCommand _command;
        private SqlStatementCollection _collectionToRemoveStatementFrom;
        private SqlStatement _statementToRemove;

        [SetUp]
        public void Setup()
        {
            _collectionToRemoveStatementFrom = MockRepository.GenerateMock<SqlStatementCollection>();
            _statementToRemove = MockRepository.GenerateMock<SqlStatement>();

            _command = new RemoveStatementCommand();
        }

        [TearDown]
        public void TearDown()
        {
            _collectionToRemoveStatementFrom.VerifyAllExpectations();
            _statementToRemove.VerifyAllExpectations();
        }

        [Test]
        public void RemoveSelectedStatementWithValidRequest()
        {
            StatementMoveRequest moveRequest = new StatementMoveRequest
            {
                CollectionToMoveStatementIn = _collectionToRemoveStatementFrom,
                StatementToMove = _statementToRemove
            };

            _collectionToRemoveStatementFrom.Expect(sc => sc.CanRemove(_statementToRemove)).Return(true);
            _collectionToRemoveStatementFrom.Expect(sc => sc.Remove(_statementToRemove));
            

            _command.Execute(moveRequest);
            _collectionToRemoveStatementFrom.AssertWasCalled(sc => sc.SetCanMoveUpDownOnAllStatements());
        }

        [Test]
        public void DoesNotRemoveSelectedStatementWithValidRequestWhenStatementCantBeRemoved()
        {
            StatementMoveRequest moveRequest = new StatementMoveRequest
            {
                CollectionToMoveStatementIn = _collectionToRemoveStatementFrom,
                StatementToMove = _statementToRemove
            };

            _collectionToRemoveStatementFrom.Expect(sc => sc.CanRemove(_statementToRemove)).Return(false);


            _command.Execute(moveRequest);
            _collectionToRemoveStatementFrom.AssertWasNotCalled(sc => sc.Remove(Arg<SqlStatement>.Is.Anything));
            _collectionToRemoveStatementFrom.AssertWasCalled(sc => sc.SetCanMoveUpDownOnAllStatements());
        }

        [Test]
        public void RemoveSelectedStatementWithInValidRequestDoesNothing()
        {
            _command.Execute(null);
        }

        [Test]
        public void CanExecuteAlwaysReturnsTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }
    }
}