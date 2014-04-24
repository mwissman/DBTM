using DBTM.Domain.Entities;

namespace DBTM.Application.Commands
{
    public class StatementMoveRequest
    {
        public SqlStatementCollection CollectionToMoveStatementIn { get; set; }
        public SqlStatement StatementToMove { get; set; }

    }
}