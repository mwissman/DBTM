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

    public class GenerateDescriptions : IMigrator
    {
        public void Migrate(Database database)
        {
            database.Versions.ForEach(v =>
            {
                GenerateDescription(v.PreDeploymentStatements);
                GenerateDescription(v.PostDeploymentStatements);
            });
        }

        private void GenerateDescription(SqlStatementCollection sqlStatementCollection)
        {
           sqlStatementCollection.Where(s=>s.Description=="---").ForEach(s =>
           {
               var lines=s.UpgradeSQL.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);

               if (lines.Length > 0)
               {
                   s.Description = lines[0].Truncate(50);
               }

           });
        }
    }

    public class RemoveAnsiNullStatements : IMigrator
    {
        public void Migrate(Database database)
        {
            database.Versions.ForEach(v =>
            {
                RemoveAnsiNullStatement(v.PreDeploymentStatements);
                RemoveAnsiNullStatement(v.PostDeploymentStatements);
            });
        }

        private void RemoveAnsiNullStatement(SqlStatementCollection sqlStatementCollection)
        {
            var toRemove=sqlStatementCollection.Where(s => s.UpgradeSQL.Contains("SET ANSI_NULLS ON")).ToList();
            toRemove.ForEach(sqlStatementCollection.Remove);
        }
    }

    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}