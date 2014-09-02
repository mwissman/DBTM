using System;
using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class RemoveStatementCommand : MoveStatementBaseCommand
    {
        protected override Action<SqlStatementCollection, SqlStatement> CollectionMoveAction
        {
            get
            {
                return (c, s) =>
                {
                    if (c.CanRemove(s))
                    {
                        c.Remove(s);
                    }
                };
            }
        }
    }
}