using System;
using System.Text;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class CompiledRollbackSqlTests
    {
        [Test]
        public void ComposeRollbackSqlReplacesCrossDatabasePlaceHolder()
        {
            var rollbackSql = new CompiledRollbackSql("SELECT * FROM [{dbx:RedboxServerDB}]..KioskClient; SELECT * FROM [{dbx:Promotions}]..PromotionCampaigns", "TestBranch_",Guid.Empty,SqlStatementType.PreDeployment, 0, false);

            string expectedRollbackSql = "SELECT * FROM [TestBranch_RedboxServerDB]..KioskClient; SELECT * FROM [TestBranch_Promotions]..PromotionCampaigns\r\n";

            Assert.AreEqual(expectedRollbackSql, rollbackSql.ToString());
        }

        [Test]
        public void ComposeRollbackSqlReplacesCrossDatabasePlaceHolderWithVersionHistory()
        {
            var rollbackSql = new CompiledRollbackSql("SELECT * FROM [{dbx:RedboxServerDB}]..KioskClient; SELECT * FROM [{dbx:Promotions}]..PromotionCampaigns", "TestBranch_", Guid.Empty, SqlStatementType.PreDeployment, 0, true);

            var expectedRollbackSql = new StringBuilder();

            expectedRollbackSql.AppendFormat("   EXEC ('SELECT * FROM [TestBranch_RedboxServerDB]..KioskClient; SELECT * FROM [TestBranch_Promotions]..PromotionCampaigns');\r\n");
            expectedRollbackSql.AppendLine();
            expectedRollbackSql.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Rollback';\r\n", 0, Guid.Empty, SqlStatementType.PreDeployment.ToString());

            Assert.AreEqual(expectedRollbackSql.ToString(), rollbackSql.ToString());
        }

        [TestCase(SqlStatementType.PreDeployment)]
        [TestCase(SqlStatementType.PostDeployment)]
        [TestCase(SqlStatementType.Backfill)]
        public void ComposeRollbackSqlCompilesScriptWithVersionHistoryInHistoryModeForAllTypes(SqlStatementType sqlStatementType)
        {
            Guid statementId = Guid.NewGuid();
            int versionNumber = 5;
            var rollbackSql = new CompiledRollbackSql("someSql ' with single quote", "",statementId,sqlStatementType,versionNumber, true);

            var expectedSqlBuilder = new StringBuilder();

            expectedSqlBuilder.AppendFormat("   EXEC ('someSql '' with single quote');\r\n");
            expectedSqlBuilder.AppendLine();
            expectedSqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Rollback';\r\n", versionNumber, statementId, sqlStatementType.ToString());

            Assert.AreEqual(expectedSqlBuilder.ToString(), rollbackSql.ToString());

        }

        [Test]
        public void ComposeRollbackSqlCompilesScriptWithoutVersionHistoryInNoHistoryMode()
        {
            Guid statementId = Guid.NewGuid();
            var sqlStatementType = SqlStatementType.PreDeployment;
            int versionNumber = 5;
            var rollbackSql = new CompiledRollbackSql("someSql ' with single quote", "", statementId, sqlStatementType, versionNumber, false);

            string expectedRollbackSql = "someSql ' with single quote\r\n";

            Assert.AreEqual(expectedRollbackSql, rollbackSql.ToString());
        }
    }
}