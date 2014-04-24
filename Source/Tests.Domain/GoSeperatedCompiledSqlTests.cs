using DBTM.Domain;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class GoSeperatedCompiledSqlTests
    {
        [TestCase("go")]
        [TestCase("Go")]
        [TestCase("gO")]
        [TestCase("GO")]
        public void UpgradeSplitsOnGoStatement(string go)
        {
            GoSeperatedCompiledSql compiledSql = new GoSeperatedCompiledSql("update1 " + go + " update2", "");


            Assert.AreEqual("update1 " + go + " update2", compiledSql.Upgrade);

            CollectionAssert.AreEquivalent(new[] { "update1 ", " update2" }, compiledSql.UpgradeStatements);
        }

        [TestCase("go")]
        [TestCase("Go")]
        [TestCase("gO")]
        [TestCase("GO")]
        public void RollbackSplitsOnGoStatement(string go)
        {
            GoSeperatedCompiledSql compiledSql = new GoSeperatedCompiledSql("", "rollback1 " + go + " rollback2");


            Assert.AreEqual("rollback1 " + go + " rollback2", compiledSql.Rollback);

            CollectionAssert.AreEquivalent(new[] { "rollback1 ", " rollback2" }, compiledSql.RollbackStatements);
        }

    }
}