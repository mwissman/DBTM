using System;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Domain.DatabaseTests
{

    [TestFixture]
    public partial class DatabaseTests
    {
        [TestFixture]
        public class WhenCompilingScriptsForTheWholeDatabase
        {
            [SetUp]
            public void Setup()
            {

            }

            [TearDown]
            public void TearDown()
            {

            }

            [Test]
            public void ConcantenatesPreAndPostDeploymentScriptsTogether()
            {

                TestableCompiledVersionSql version1CompilePreDeployment = new TestableCompiledVersionSql("--version 1 pre deployment upgrade", "--version 1 pre deployment rollback");
                TestableCompiledVersionSql version1CompilePostDeployment = new TestableCompiledVersionSql("--version 1 post deployment upgrade", "--version 1 post deployment rollback");
                TestableCompiledVersionSql version2CompilePreDeployment = new TestableCompiledVersionSql("--version 2 pre deployment upgrade", "--version 2 pre deployment rollback");
                TestableCompiledVersionSql version2CompilePostDeployment = new TestableCompiledVersionSql("--version 2 post deployment upgrade", "--version 2 post deployment rollback");
                TestableCompiledVersionSql version3CompilePreDeployment = new TestableCompiledVersionSql("--version 3 pre deployment upgrade", "--version 3 pre deployment rollback");
                TestableCompiledVersionSql version3CompilePostDeployment = new TestableCompiledVersionSql("--version 4 post deployment upgrade", "--version 3 post deployment rollback");

                string expectedUpgrade = string.Format(@"-----------------------------------------------------------------
-- {0}

--DBTM upgrade history here --

{1}
{2}
{3}
{4}
{5}
{6}
", "TestDatabase", 
    version1CompilePreDeployment.Upgrade, version1CompilePostDeployment.Upgrade,
    version2CompilePreDeployment.Upgrade, version2CompilePostDeployment.Upgrade,
    version3CompilePreDeployment.Upgrade, version3CompilePostDeployment.Upgrade
    
    );
                string expectedRollback = string.Format(@"-----------------------------------------------------------------
-- {0}

{1}
{2}
{3}
{4}
{5}
{6}

--DBTM rollback history here --

", "TestDatabase",
    version3CompilePostDeployment.Rollback, version3CompilePreDeployment.Rollback,
    version2CompilePostDeployment.Rollback, version2CompilePreDeployment.Rollback,
    version1CompilePostDeployment.Rollback, version1CompilePreDeployment.Rollback

    );

                DatabaseVersion version1 = MockRepository.GenerateMock<DatabaseVersion>(1, DateTime.Now);
                DatabaseVersion version2 = MockRepository.GenerateMock<DatabaseVersion>(2, DateTime.Now);
                DatabaseVersion version3 = MockRepository.GenerateMock<DatabaseVersion>(3, DateTime.Now);
                
                version1.Stub(v => v.CompileSql("", SqlStatementType.PreDeployment,false)).Return(version1CompilePreDeployment);
                version1.Stub(v => v.CompileSql("", SqlStatementType.PostDeployment, false)).Return(version1CompilePostDeployment);

                version2.Stub(v => v.IsBaseline).Return(true);
                version2.Stub(v => v.CompileSql("", SqlStatementType.PreDeployment, true)).Return(version2CompilePreDeployment);
                version2.Stub(v => v.CompileSql("", SqlStatementType.PostDeployment, true)).Return(version2CompilePostDeployment);

                version3.Stub(v => v.CompileSql("", SqlStatementType.PreDeployment, true)).Return(version3CompilePreDeployment);
                version3.Stub(v => v.CompileSql("", SqlStatementType.PostDeployment, true)).Return(version3CompilePostDeployment);

                Database database = new Database("TestDatabase");
                
                database.Versions.Add(version1);
                database.Versions.Add(version2);
                database.Versions.Add(version3);

                var compiled = database.CompileAllVersions(new CompiledSql("--DBTM upgrade history here --", "--DBTM rollback history here --"));
                
                Assert.AreEqual(expectedUpgrade,compiled.Upgrade);
                Assert.AreEqual(expectedRollback,compiled.Rollback);

                version1.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PreDeployment,false));
                version1.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PostDeployment, false));
                
                version2.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PreDeployment,true));
                version2.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PostDeployment, true));

                version3.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PreDeployment, true));
                version3.AssertWasCalled(v => v.CompileSql("", SqlStatementType.PostDeployment, true));

            }

        }
    }

    public class TestableCompiledVersionSql : CompiledVersionSql
    {
        private readonly string _upgradeSql;
        private readonly string _rollbackSql;

        public TestableCompiledVersionSql(string upgradeSql, string rollbackSql)
            : base(0, SqlStatementType.PreDeployment)
        {
            _upgradeSql = upgradeSql;
            _rollbackSql = rollbackSql;
        }

        public override string Upgrade
        {
            get { return _upgradeSql; }
        }
        public override string Rollback
        {
            get
            {
                return _rollbackSql;
            }
        }

    }
}