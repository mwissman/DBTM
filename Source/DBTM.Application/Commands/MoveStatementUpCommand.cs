using System;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class MoveStatementUpCommand : MoveStatementBaseCommand
    {
        protected override Action<SqlStatementCollection, SqlStatement> CollectionMoveAction
        {
            get
            {
                return (c, s) =>
                           {
                               if (c.CanMoveUp(s))
                               {
                                   c.MoveItemUp(s);
                               }
                           };
            }
        }
    }

}