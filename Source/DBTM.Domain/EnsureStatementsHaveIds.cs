using System;
using System.Collections.Generic;
using System.Linq;
using DBTM.Domain.Entities;

namespace DBTM.Domain
{
    public class EnsureStatementsHaveIds : IMigrator
    {
        private readonly IGuidFactory _guidFactory;

        public EnsureStatementsHaveIds(IGuidFactory guidFactory)
        {
            _guidFactory = guidFactory;
        }

        public void Migrate(Database database)
        {
            database.Versions.ForEach(v =>
                {
                    FillInMissingIds(v.PreDeploymentStatements);
                    FillInMissingIds(v.PostDeploymentStatements);
                });
        }

        private void FillInMissingIds(IEnumerable<SqlStatement> sqlStatementCollection)
        {
            sqlStatementCollection.Where(s=>s.Id==  Guid.Empty).ForEach(s=>s.Id=_guidFactory.Create());
        }
    }
}