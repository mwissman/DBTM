using System;
using System.Windows.Input;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public abstract class MoveStatementBaseCommand : ICommand
    {
        protected abstract Action<SqlStatementCollection, SqlStatement> CollectionMoveAction { get; }

        public void Execute(object parameter)
        {
            var request = parameter as StatementMoveRequest;
            if (request != null)
            {
                CollectionMoveAction.Invoke(request.CollectionToMoveStatementIn,request.StatementToMove);
                request.CollectionToMoveStatementIn.SetCanMoveUpDownOnAllStatements();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}