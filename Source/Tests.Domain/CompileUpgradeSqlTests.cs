using System;
using System.Text;
using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class CompileUpgradeSqlTests
    {
        [Test]
        public void CompileWorksIfNullSql()
        {
            var upgradeSql = new CompiledUpgradeSql(null, "TestBranch_", Guid.Empty, SqlStatementType.PreDeployment, 0, false);

            string expectedUpgradeSql = "\r\n";

            Assert.AreEqual(expectedUpgradeSql, upgradeSql.ToString());
        }

        [Test]
        public void ComposeUpdateSqlReplacesCrossDatabasePlaceHolder()
        {
            var upgradeSql = new CompiledUpgradeSql("SELECT * FROM [{dbx:DatabaseName}]..KioskClient; SELECT * FROM [{dbx:Promotions}]..PromotionCampaigns", "TestBranch_", Guid.Empty, SqlStatementType.PreDeployment, 0, false);

            string expectedUpgradeSql = "SELECT * FROM [TestBranch_DatabaseName]..KioskClient; SELECT * FROM [TestBranch_Promotions]..PromotionCampaigns\r\n";

            Assert.AreEqual(expectedUpgradeSql, upgradeSql.ToString());
        }

        [TestCase(SqlStatementType.PreDeployment)]
        [TestCase(SqlStatementType.PostDeployment)]
        public void ComposeUpdateSqlCompilesScriptWithVersionHistoryInHistoryModeForPreAndPostSqlTypes(SqlStatementType sqlStatementType)
        {
            Guid statementId = Guid.NewGuid();
            int versionNumber = 5;
            var upgradeSql = new CompiledUpgradeSql("someSql ' with single quote", "", statementId, sqlStatementType, versionNumber, true);

            var expectedSqlBuilder = new StringBuilder();


            expectedSqlBuilder.AppendLine("DECLARE @canrun BIT = 0;");
            expectedSqlBuilder.AppendFormat("EXEC @canrun=[DBTM].sp_CanRunStatementUpgrade '{0}';\r\n", statementId);
            expectedSqlBuilder.AppendLine("IF (@canrun)=1");
            expectedSqlBuilder.AppendLine("BEGIN");
            expectedSqlBuilder.AppendFormat("    PRINT 'Running Statement {0}.';\r\n", statementId);
            expectedSqlBuilder.AppendLine();
            expectedSqlBuilder.AppendFormat("    EXEC ('someSql '' with single quote');\r\n");
            expectedSqlBuilder.AppendLine();
            expectedSqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Upgrade';\r\n", versionNumber, statementId, sqlStatementType.ToString());
            expectedSqlBuilder.AppendLine("END");
            expectedSqlBuilder.AppendLine("ELSE");
            expectedSqlBuilder.AppendLine("BEGIN");
            expectedSqlBuilder.AppendFormat("	PRINT 'Statement {0} has already been executed.';\r\n", statementId);
            expectedSqlBuilder.AppendLine("END");

            Assert.AreEqual(expectedSqlBuilder.ToString(), upgradeSql.ToString());

        }

        [TestCase]
        public void ComposeUpdateSqlCompilesScriptWithVersionHistoryInHistoryModeForBackfillSqlTypes()
        {
            Guid statementId = Guid.NewGuid();
            int versionNumber = 5;
            SqlStatementType sqlStatementType = SqlStatementType.Backfill;

            var upgradeSql = new CompiledUpgradeSql("someSql ' with single quote", "", statementId, sqlStatementType, versionNumber, true);

            var expectedSqlBuilder = new StringBuilder();

            expectedSqlBuilder.AppendFormat("    EXEC ('someSql '' with single quote');\r\n");
            expectedSqlBuilder.AppendLine();
            expectedSqlBuilder.AppendFormat("    EXECUTE [DBTM].[sp_RecordStatementExecuted] {0},'{1}','{2}','Upgrade';\r\n", versionNumber, statementId, sqlStatementType.ToString());

            Assert.AreEqual(expectedSqlBuilder.ToString(), upgradeSql.ToString());

        }

        [Test]
        public void ComposeUpdateSqlCompilesScriptWithoutVersionHistoryInNoHistoryMode()
        {
            Guid statementId = Guid.NewGuid();
            var sqlStatementType = SqlStatementType.PreDeployment;
            int versionNumber = 5;
            var upgradeSql = new CompiledUpgradeSql("someSql ' with single quote", "", statementId, sqlStatementType, versionNumber, false);

            string expectedUpgradeSql = "someSql ' with single quote\r\n";

            Assert.AreEqual(expectedUpgradeSql, upgradeSql.ToString());
        }
    }
}