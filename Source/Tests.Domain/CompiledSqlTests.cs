using DBTM.Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class CompiledSqlTests
    {
        [Test]
        public void UpgradeAndRollbackStatementListsHaveOneItemEach()
        {
            CompiledSql compiledSql=new CompiledSql("upgrade","rollback");

            Assert.AreEqual("upgrade", compiledSql.Upgrade);
            Assert.AreEqual("rollback", compiledSql.Rollback);

            CollectionAssert.AreEquivalent(new[]{"upgrade"},compiledSql.UpgradeStatements);
            CollectionAssert.AreEquivalent(new[] { "rollback" }, compiledSql.RollbackStatements);
        }
    }
}