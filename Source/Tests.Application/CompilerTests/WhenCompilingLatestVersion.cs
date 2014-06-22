using System;
using DBTM.Application;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.CompilerTests
{
    [TestFixture]
    public class WhenCompilingLatestVersion
    {
        private const string _preDeploymentUpgradePath = "PreDeploymentUpgrade.sql";
        private const string _preDeploymentRollbackPath = "PreDeploymentRollback.sql";
        private const string _databasePrefix = "DatabasePrefix_";
        private const string _compiledSqlFolderPath = "c:\\folder\\path";

        private Compiler _compiler;

        private ISqlFileWriter _sqlFileWriter;
        private DatabaseVersion _databaseVersion;
        private SqlStatementCollection _preDeploymentStatments;
        private SqlStatementCollection _postDeploymentStatments;
        private ISqlScriptRepository _sqlScriptRepository;

        [SetUp]
        public void Setup()
        {

            _sqlFileWriter = MockRepository.GenerateMock<ISqlFileWriter>();
            _sqlScriptRepository = MockRepository.GenerateMock<ISqlScriptRepository>();

            _databaseVersion = MockRepository.GenerateMock<DatabaseVersion>(2, DateTime.Now);

            _preDeploymentStatments = new SqlStatementCollection();
            _postDeploymentStatments = new SqlStatementCollection();

            _databaseVersion.Stub(dv => dv.PreDeploymentStatements).Return(_preDeploymentStatments);
            _databaseVersion.Stub(dv => dv.PostDeploymentStatements).Return(_postDeploymentStatments);
            _databaseVersion.Stub(dv => dv.HasStatements).Return(true);
            _databaseVersion.VersionNumber = 2;
            _databaseVersion.IsBaseline = false;

            _compiler = new Compiler(_sqlFileWriter, _sqlScriptRepository);
        }

        [TearDown]
        public void TearDown()
        {
            _sqlFileWriter.VerifyAllExpectations();
            _databaseVersion.VerifyAllExpectations();
            _sqlScriptRepository.VerifyAllExpectations();
        }

        [Test]
        public void PicksTheMaxVersionToCompile()
        {
            CompiledVersionSql preDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);

            var database = new Database("test");
            database.AddChangeset();
            database.Versions.Add(_databaseVersion);

            _preDeploymentStatments.Add(new SqlStatement("", "", ""));

            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PreDeployment, true)).Return(preDeploymentCompiledSql);

            _compiler.CompileLatestVersion(database, _compiledSqlFolderPath, _databasePrefix);

            _sqlFileWriter.AssertWasCalled(sfw => sfw.Write(preDeploymentCompiledSql,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentUpgradePath,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentRollbackPath));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DoesNothingIfVersionHasNoStatements(bool isBaseLine)
        {
            var database = new Database("test");
            var version = database.AddChangeset();
            version.IsBaseline = isBaseLine;

            _compiler.CompileLatestVersion(database, "", "");

            _sqlFileWriter.AssertWasNotCalled(w => w.Write(Arg<CompiledVersionSql>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [Test]
        public void IncludesDBTMSqlFunctionsIfMaxVersionIsTheBaseLine()
        {
            CompiledVersionSql preDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            var historySql = new CompiledSql("", "");

            var database = new Database("test");
            database.Versions.Add(_databaseVersion);
            _databaseVersion.Stub(v => v.IsBaseline).Return(true);

            _preDeploymentStatments.Add(new SqlStatement("", "", ""));

            _sqlScriptRepository.Expect(r => r.LoadHistroySql()).Return(historySql);
            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PreDeployment, true)).Return(preDeploymentCompiledSql);

            _compiler.CompileLatestVersion(database, _compiledSqlFolderPath, _databasePrefix);

            var expectedHistoryUpgradePath = _compiledSqlFolderPath + "\\0_DBTMHistoryUpgrade.sql";
            var expectedHistoryRollbackPath = _compiledSqlFolderPath + "\\0_DBTMHistoryRollback.sql";

            _sqlFileWriter.AssertWasCalled(w => w.Write(historySql, expectedHistoryUpgradePath, expectedHistoryRollbackPath));
            _sqlFileWriter.AssertWasCalled(sfw => sfw.Write(preDeploymentCompiledSql,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentUpgradePath,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentRollbackPath));
        }

        [Test]
        public void DoesNotIncludeHistoryWhenDatabaseDoesNotHaveABaselineVersion()
        {
            CompiledVersionSql preDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);

            var database = new Database("test");
            database.Versions.Add(_databaseVersion);
            _databaseVersion.IsBaseline = false;

            _preDeploymentStatments.Add(new SqlStatement("", "", ""));

            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PreDeployment, false)).Return(preDeploymentCompiledSql);

            _compiler.CompileLatestVersion(database, _compiledSqlFolderPath, _databasePrefix);

            _sqlFileWriter.AssertWasCalled(sfw => sfw.Write(preDeploymentCompiledSql,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentUpgradePath,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentRollbackPath));
        }

        [Test]
        public void IncludesHistoryWhenVersionIsAfterBaselineVersion()
        {
            CompiledVersionSql preDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);

            var database = new Database("test");
            database.Versions.Add(new DatabaseVersion(1, DateTime.Now) { IsBaseline = true });
            database.Versions.Add(_databaseVersion);
            _databaseVersion.IsBaseline = false;

            _preDeploymentStatments.Add(new SqlStatement("", "", ""));

            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PreDeployment, true)).Return(preDeploymentCompiledSql);

            _compiler.CompileLatestVersion(database, _compiledSqlFolderPath, _databasePrefix);

            _sqlFileWriter.AssertWasCalled(sfw => sfw.Write(preDeploymentCompiledSql,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentUpgradePath,
                                                 _compiledSqlFolderPath + "\\" + _preDeploymentRollbackPath));
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public void CompilesPreAndPostDeploymentScriptsWhenStatementsExist(bool hasPreDeploymentStatements, bool hasPostDeploymentStatements)
        {
            CompiledVersionSql preDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);
            CompiledVersionSql postDeploymentCompiledSql = new CompiledVersionSql(0, SqlStatementType.PreDeployment);

            string postDeploymentUpgradePath = "PostDeploymentUpgrade.sql";
            string postDeploymentRollbackPath = "PostDeploymentRollback.sql";

            if (hasPreDeploymentStatements)
            {
                _preDeploymentStatments.Add(new SqlStatement("", "", ""));
            }

            if (hasPostDeploymentStatements)
            {
                _postDeploymentStatments.Add(new SqlStatement("", "", ""));
            }


            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PreDeployment, false)).
                Return(preDeploymentCompiledSql).Repeat.Times(hasPreDeploymentStatements ? 1 : 0);

            _databaseVersion.Expect(dv => dv.CompileSql(_databasePrefix, SqlStatementType.PostDeployment, false)).
                Return(postDeploymentCompiledSql).Repeat.Times(hasPostDeploymentStatements ? 1 : 0);


            _sqlFileWriter.Expect(sfw => sfw.Write(preDeploymentCompiledSql,
                                                   _compiledSqlFolderPath + "\\" + _preDeploymentUpgradePath,
                                                   _compiledSqlFolderPath + "\\" + _preDeploymentRollbackPath)).
                Repeat.Times(hasPreDeploymentStatements ? 1 : 0);

            _sqlFileWriter.Expect(sfw => sfw.Write(postDeploymentCompiledSql,
                                                   _compiledSqlFolderPath + "\\" + postDeploymentUpgradePath,
                                                   _compiledSqlFolderPath + "\\" + postDeploymentRollbackPath)).
                Repeat.Times(hasPostDeploymentStatements ? 1 : 0);

            var database = new Database("test");
            database.Versions.Add(_databaseVersion);

            _compiler.CompileLatestVersion(database, _compiledSqlFolderPath, _databasePrefix);
        }

    }

}
