using System;
using System.Collections.Generic;

namespace DBTM.Domain.Entities
{
    public class EmptyDatabaseVersion : DatabaseVersion
    {
        public EmptyDatabaseVersion() : base(int.MinValue, DateTime.MinValue)
        {
            PreDeploymentStatements = new SqlStatementCollection();
        }

        public override bool CanBeBuilt()
        {
            return false;
        }
    }
}