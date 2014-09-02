using DBTM.Application.Commands;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class MoveStatementDownCommandTests
    {
        private MoveStatementDownCommand _command;
        private SqlStatementCollection _collectionToMoveStatementIn;
        private SqlStatement _statementToMove;

        [SetUp]
        public void Setup()
        {
            _collectionToMoveStatementIn = MockRepository.GenerateMock<SqlStatementCollection>();
            _statementToMove = MockRepository.GenerateMock<SqlStatement>();

            _command = new MoveStatementDownCommand();
        }

        [TearDown]
        public void TearDown()
        {
            _collectionToMoveStatementIn.VerifyAllExpectations();
            _statementToMove.VerifyAllExpectations();
        }

        [Test]
        public void MovesSelectedStatementUpWithValidMoveRequest()
        {
            StatementMoveRequest moveRequest = new StatementMoveRequest
                                                   {
                                                       CollectionToMoveStatementIn = _collectionToMoveStatementIn,
                                                       StatementToMove = _statementToMove
                                                   };

            _collectionToMoveStatementIn.Expect(sc => sc.CanMoveDown(_statementToMove)).Return(true);
            _collectionToMoveStatementIn.Expect(sc => sc.MoveItemDown(_statementToMove));

            _command.Execute(moveRequest);

            _collectionToMoveStatementIn.AssertWasCalled(sc => sc.SetCanMoveUpDownOnAllStatements());
        }

        [Test]
        public void DoesNotMovesSelectedStatementUpWithValidMoveRequestWhenStatementCantMoveDown()
        {
            StatementMoveRequest moveRequest = new StatementMoveRequest
            {
                CollectionToMoveStatementIn = _collectionToMoveStatementIn,
                StatementToMove = _statementToMove
            };

            _collectionToMoveStatementIn.Expect(sc => sc.CanMoveDown(_statementToMove)).Return(false);
            

            _command.Execute(moveRequest);
            _collectionToMoveStatementIn.AssertWasNotCalled(sc => sc.MoveItemDown(Arg<SqlStatement>.Is.Anything));
            _collectionToMoveStatementIn.AssertWasCalled(sc => sc.SetCanMoveUpDownOnAllStatements());
        }

        [Test]
        public void MovesSelectedStatementUpWithInValidMoveRequestDoesNothing()
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