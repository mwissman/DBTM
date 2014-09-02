using System;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain.DatabaseVersionTests
{
    [TestFixture]
    public partial class DatabaseVersionTests
    {

        [TestFixture]
        public class WhenCompilingSql
        {

            #region Expected Sql

            private string EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_UPGRADE = @"

{0}
GO

----------------------------------------------------------------

{1}
GO

----------------------------------------------------------------

{2}
GO

----------------------------------------------------------------
";

            private string EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_ROLLBACK = @"

{2}
GO

----------------------------------------------------------------

{1}
GO

----------------------------------------------------------------

{0}
GO

----------------------------------------------------------------
";

            private string EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_UPGRADE_WITH_HISTORY = @"

DECLARE @canrun BIT = 0;
EXEC @canrun=[DBTM].sp_CanRunStatementUpgrade '{3}';
IF (@canrun)=1
BEGIN
    PRINT 'Running Statement {3}.';

    EXEC ('{0}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{3}','{6}','Upgrade';
END
ELSE
BEGIN
	PRINT 'Statement {3} has already been executed.';
END
GO

----------------------------------------------------------------

DECLARE @canrun BIT = 0;
EXEC @canrun=[DBTM].sp_CanRunStatementUpgrade '{4}';
IF (@canrun)=1
BEGIN
    PRINT 'Running Statement {4}.';

    EXEC ('{1}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{4}','{6}','Upgrade';
END
ELSE
BEGIN
	PRINT 'Statement {4} has already been executed.';
END
GO

----------------------------------------------------------------

DECLARE @canrun BIT = 0;
EXEC @canrun=[DBTM].sp_CanRunStatementUpgrade '{5}';
IF (@canrun)=1
BEGIN
    PRINT 'Running Statement {5}.';

    EXEC ('{2}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{5}','{6}','Upgrade';
END
ELSE
BEGIN
	PRINT 'Statement {5} has already been executed.';
END
GO

----------------------------------------------------------------
";

            private string EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_ROLLBACK_WITH_HISTORY  = @"

   EXEC ('{2}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{5}','{6}','Rollback';
GO

----------------------------------------------------------------

   EXEC ('{1}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{4}','{6}','Rollback';
GO

----------------------------------------------------------------

   EXEC ('{0}');

    EXECUTE [DBTM].[sp_RecordStatementExecuted] 2,'{3}','{6}','Rollback';
GO

----------------------------------------------------------------
";

            #endregion 

            [TestCase(SqlStatementType.PreDeployment)]
            [TestCase(SqlStatementType.PostDeployment)]
            public void CompileSqlCombinesAllStatementsInOrderUpgradeAscendingRollBackDescendingDoesNotIncludeHistory(SqlStatementType sqlStatementType)
            {
                string databasePrefix = "databasePrefix_";

                var versionId = 2;
                DatabaseVersion databaseVersion = new DatabaseVersion(versionId, DateTime.Now);

                var upgradeSql1 = "some sql 1";
                var upgradeSql2 = "some sql 2";
                var upgradeSql3 = "some sql 3";
                var rollbackSql1 = "some other sql";
                var rollbackSql2 = string.Empty;
                var rollbackSql3 = "some more sql";

                SqlStatement sqlStatement1 = MockRepository.GenerateMock<SqlStatement>("desc1", upgradeSql1, rollbackSql1);
                sqlStatement1.Stub(s => s.UpgradeSQL).Return(upgradeSql1);
                sqlStatement1.Stub(s => s.RollbackSQL).Return(rollbackSql1);

                SqlStatement sqlStatement2 = MockRepository.GenerateMock<SqlStatement>("desc2", upgradeSql2, rollbackSql2);
                sqlStatement2.Stub(s => s.UpgradeSQL).Return(upgradeSql2);
                sqlStatement2.Stub(s => s.RollbackSQL).Return(rollbackSql2);

                SqlStatement sqlStatement3 = MockRepository.GenerateMock<SqlStatement>("desc3", upgradeSql3, rollbackSql3);
                sqlStatement3.Stub(s => s.UpgradeSQL).Return(upgradeSql3);
                sqlStatement3.Stub(s => s.RollbackSQL).Return(rollbackSql3);

                SqlStatementCollection statements = GetStatements(sqlStatementType, databaseVersion);

                statements.Add(sqlStatement1);
                statements.Add(sqlStatement2);
                statements.Add(sqlStatement3);

                var actualSql = databaseVersion.CompileSql(databasePrefix, sqlStatementType,false);

                string expectedUpgrade =
                    (string.Format(@"-- Version {0} - {1} ", versionId, sqlStatementType).PadRight(65, '-') +
                    string.Format(EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_UPGRADE, upgradeSql1, upgradeSql2, upgradeSql3)).FixNewlines();

                string expectedRollback =
                    (string.Format(@"-- Version {0} - {1} ", versionId, sqlStatementType).PadRight(65, '-') +
                    string.Format(EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_ROLLBACK, rollbackSql1, rollbackSql2, rollbackSql3)).FixNewlines();
                
                Assert.AreEqual(expectedUpgrade, actualSql.Upgrade.ToString());
                Assert.AreEqual(expectedRollback, actualSql.Rollback.ToString());

                sqlStatement1.VerifyAllExpectations();
                sqlStatement2.VerifyAllExpectations();
                sqlStatement3.VerifyAllExpectations();
            }

            [TestCase(SqlStatementType.PreDeployment)]
            [TestCase(SqlStatementType.PostDeployment)]
            public void CompileSqlCombinesAllStatementsInOrderUpgradeAscendingRollBackDescendingDoesIncludeHistory(SqlStatementType sqlStatementType)
            {
                string databasePrefix = "databasePrefix_";

                var versionId = 2;
                DatabaseVersion databaseVersion = new DatabaseVersion(versionId, DateTime.Now);

                var upgradeSql1 = "some sql ' 1";
                var rollbackSql1 = "some other ' sql";
                Guid statement1Id = Guid.NewGuid();

                var upgradeSql2 = "some sql \r\n2";
                var rollbackSql2 = string.Empty;
                Guid statement2Id = Guid.NewGuid();

                var upgradeSql3 = "some sql 3";
                var rollbackSql3 = "some more sql";
                Guid statement3Id = Guid.NewGuid();

                SqlStatement sqlStatement1 = MockRepository.GenerateMock<SqlStatement>("desc1", upgradeSql1, rollbackSql1);
                sqlStatement1.Stub(s=>s.Id).Return(statement1Id);
                sqlStatement1.Stub(s => s.UpgradeSQL).Return(upgradeSql1);
                sqlStatement1.Stub(s => s.RollbackSQL).Return(rollbackSql1);

                SqlStatement sqlStatement2 = MockRepository.GenerateMock<SqlStatement>("desc2", upgradeSql2, rollbackSql2);
                sqlStatement2.Stub(s => s.Id).Return(statement2Id);
                sqlStatement2.Stub(s => s.UpgradeSQL).Return(upgradeSql2);
                sqlStatement2.Stub(s => s.RollbackSQL).Return(rollbackSql2);

                SqlStatement sqlStatement3 = MockRepository.GenerateMock<SqlStatement>("desc3", upgradeSql3, rollbackSql3);
                sqlStatement3.Stub(s => s.Id).Return(statement3Id);
                sqlStatement3.Stub(s => s.UpgradeSQL).Return(upgradeSql3);
                sqlStatement3.Stub(s => s.RollbackSQL).Return(rollbackSql3);

                SqlStatementCollection statements = GetStatements(sqlStatementType, databaseVersion);

                statements.Add(sqlStatement1);
                statements.Add(sqlStatement2);
                statements.Add(sqlStatement3);

                var actualSql = databaseVersion.CompileSql(databasePrefix, sqlStatementType, true);

                string expectedUpgrade =
                    string.Format(@"-- Version {0} - {1} ", versionId, sqlStatementType).PadRight(65, '-') +
                    string.Format(EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_UPGRADE_WITH_HISTORY, upgradeSql1.Replace("'", "''"), upgradeSql2.Replace("'", "''"), upgradeSql3.Replace("'", "''"), statement1Id, statement2Id, statement3Id, sqlStatementType).FixNewlines();

                string expectedRollback =
                    string.Format(@"-- Version {0} - {1} ", versionId, sqlStatementType).PadRight(65, '-') +
                    string.Format(EXPECTED_SQL_TEMPLATE_DIFFERENT_ORDERS_ROLLBACK_WITH_HISTORY, rollbackSql1.Replace("'", "''"), rollbackSql2.Replace("'", "''"), rollbackSql3.Replace("'", "''"), statement1Id, statement2Id, statement3Id, sqlStatementType).FixNewlines();

                Assert.AreEqual(expectedUpgrade, actualSql.Upgrade.ToString());
                Assert.AreEqual(expectedRollback, actualSql.Rollback.ToString());

                sqlStatement1.VerifyAllExpectations();
                sqlStatement2.VerifyAllExpectations();
                sqlStatement3.VerifyAllExpectations();
            }

            private SqlStatementCollection GetStatements(SqlStatementType sqlStatementType, DatabaseVersion databaseVersion)
            {
                SqlStatementCollection statements;

                switch (sqlStatementType)
                {
                    case SqlStatementType.PreDeployment:
                        statements = databaseVersion.PreDeploymentStatements;
                        break;
                    case SqlStatementType.PostDeployment:
                        statements = databaseVersion.PostDeploymentStatements;
                        break;
                    default:
                        throw new ArgumentException("Unknown Sql Statement Type");
                }
                return statements;
            }

        }

    }
}