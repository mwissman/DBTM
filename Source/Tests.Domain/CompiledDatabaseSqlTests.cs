using System;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain
{
    [TestFixture]
    public class CompiledDatabaseSqlTests
    {
        private CompiledDatabaseSql _compiled;

        [SetUp]
        public void Setup()
        {
            ICompiledSql compiledHistory = new CompiledSql("history upgrade", "history rollback");
            ICompiledSql version1Pre = new CompiledSql("version 1 pre upgrade", "version 1 pre rollback");
            ICompiledSql version1Post = new CompiledSql("version 1 post upgrade", "version 1 post rollback");
            ICompiledSql version2Pre = new CompiledSql("version 2 pre upgrade", "version 2 pre rollback");
            ICompiledSql version2Post = new CompiledSql("version 2 post upgrade", "version 2 post rollback");

            ICompiledSql version3Pre = MockRepository.GenerateMock<ICompiledSql>();
            version3Pre.Stub(v => v.UpgradeStatements).Return(new[] { "version 3 pre upgrade 1", "version 3 pre upgrade 2" });
            version3Pre.Stub(v => v.RollbackStatements).Return(new[] { "version 3 pre rollback 2", "version 3 pre rollback 1" });
            version3Pre.Stub(v => v.Upgrade).Return("complete version 3 pre upgrade");
            version3Pre.Stub(v => v.Rollback).Return("complete version 3 pre rollback");

            ICompiledSql version3Post = MockRepository.GenerateMock<ICompiledSql>();
            version3Post.Stub(v => v.UpgradeStatements).Return(new[] { "version 3 post upgrade 1", "version 3 post upgrade 2" });
            version3Post.Stub(v => v.RollbackStatements).Return(new[] { "version 3 post rollback 2", "version 3 post rollback 1" });
            version3Post.Stub(v => v.Upgrade).Return("complete version 3 post upgrade");
            version3Post.Stub(v => v.Rollback).Return("complete version 3 post rollback");


            _compiled = new CompiledDatabaseSql("Foo", compiledHistory);


            _compiled.AddVersion(version1Pre, version1Post);
            _compiled.AddVersion(version2Pre, version2Post);
            _compiled.AddVersion(version3Pre, version3Post);
        }

        [Test]
        public void UpgradeCollectionHasHistoryEachVersionPreAndPostScripts()
        {
            CollectionAssert.AreEquivalent(new[]
                {
                    "history upgrade",
                    "version 1 pre upgrade",
                    "version 1 post upgrade",
                    "version 2 pre upgrade",
                    "version 2 post upgrade",
                    "version 3 pre upgrade 1",
                    "version 3 pre upgrade 2",
                    "version 3 post upgrade 1",
                    "version 3 post upgrade 2"
                }, _compiled.UpgradeStatements);

        }

        [Test]
        public void RollbackCollectionHasHistoryEachVersionPreAndPostScriptsInReverse()
        {
            CollectionAssert.AreEquivalent(new[]
                {
                    "version 3 post rollback 2",
                    "version 3 post rollback 1",
                    "version 3 pre rollback 2",
                    "version 3 pre rollback 1",
                    "version 2 post rollback",
                    "version 2 pre rollback",
                    "version 1 post rollback",
                    "version 1 pre rollback",
                    "history rollback"
                }, _compiled.RollbackStatements);

        }

        [Test]
        public void UpdateStringBuildsStringFromVersionsAndHistory()
        {
            string expectedUpdate = @"-----------------------------------------------------------------
-- Foo

history upgrade

version 1 pre upgrade
version 1 post upgrade
version 2 pre upgrade
version 2 post upgrade
complete version 3 pre upgrade
complete version 3 post upgrade
";
            var actualUpdate = _compiled.Upgrade;

            Assert.AreEqual(expectedUpdate,actualUpdate);
        }

        [Test]
        public void RollbackStringBuildsStringFromVersionsAndHistory()
        {
            string expectedRollback = @"-----------------------------------------------------------------
-- Foo

complete version 3 post rollback
complete version 3 pre rollback
version 2 post rollback
version 2 pre rollback
version 1 post rollback
version 1 pre rollback

history rollback

";
            var actualRollback = _compiled.Rollback;

            Assert.AreEqual(expectedRollback, actualRollback);
        }
    }
}