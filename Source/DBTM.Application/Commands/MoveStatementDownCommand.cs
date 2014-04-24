using System;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class MoveStatementDownCommand : MoveStatementBaseCommand
    {
        protected override Action<SqlStatementCollection, SqlStatement> CollectionMoveAction
        {
            get { return (c, s) =>
                             {
                                 if (c.CanMoveDown(s))
                                 {
                                     c.MoveItemDown(s);
                                 }
                             }; }
        }
    }
}