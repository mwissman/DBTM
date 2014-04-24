using DBTM.Application;
using DBTM.Application.Commands;
using DBTM.Application.ViewModels;
using DBTM.Domain;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.Commands
{
    [TestFixture]
    public class MoveStatementUpCommandTests
    {
        private MoveStatementUpCommand _command;
        private SqlStatementCollection _collectionToMoveStatementIn;
        private SqlStatement _statementToMove;

        [SetUp]
        public void Setup()
        {
            _collectionToMoveStatementIn = MockRepository.GenerateMock<SqlStatementCollection>();
            _statementToMove = MockRepository.GenerateMock<SqlStatement>();

            _command = new MoveStatementUpCommand();
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

            _collectionToMoveStatementIn.Expect(sc => sc.CanMoveUp(_statementToMove)).Return(true);
            _collectionToMoveStatementIn.Expect(sc => sc.MoveItemUp(_statementToMove));

            _command.Execute(moveRequest);
        }

        [Test]
        public void DoesNotMovesSelectedStatementUpWithValidMoveRequestWhenCantMoveStatementUp()
        {
            StatementMoveRequest moveRequest = new StatementMoveRequest
            {
                CollectionToMoveStatementIn = _collectionToMoveStatementIn,
                StatementToMove = _statementToMove
            };

            _collectionToMoveStatementIn.Expect(sc => sc.CanMoveUp(_statementToMove)).Return(false);
            

            _command.Execute(moveRequest);

            _collectionToMoveStatementIn.AssertWasNotCalled(sc => sc.MoveItemUp(Arg<SqlStatement>.Is.Anything));
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