using System;
using DBTM.Application;
using DBTM.Domain.Entities;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application.CompilerTests
{
    [TestFixture]
    public class WhenCompilingAll
    {
        private ISqlFileWriter _sqlFileWriter;
        private Compiler _compiler;
        private Database _database;
        private ISqlScriptRepository _sqlScriptRepository;

        [SetUp]
        public void Setup()
        {
            _sqlFileWriter = MockRepository.GenerateMock<ISqlFileWriter>();
            _database = MockRepository.GenerateMock<Database>();

            _sqlScriptRepository = MockRepository.GenerateMock<ISqlScriptRepository>();
            _compiler = new Compiler(_sqlFileWriter, _sqlScriptRepository);
        }

        [TearDown]
        public void TearDown()
        {
            _sqlFileWriter.VerifyAllExpectations();
            _sqlScriptRepository.VerifyAllExpectations();
        }

        [Test]
        public void CompilesDatabaseAndSaves()
        {
            string compileScriptPath = "c:\\path\\somewhere";
            string databaseName = "TestDatabase";
            var compiledHistorySql = new CompiledSql("", "");

            _database.Stub(d => d.DbName).Return(databaseName);
            _sqlScriptRepository.Expect(r => r.LoadHistroySql()).Return(compiledHistorySql);


            CompiledSql compiledSql = new CompiledSql("", "");
            _database.Expect(d => d.CompileAllVersions(compiledHistorySql)).Return(compiledSql);
            _sqlFileWriter.Expect(w => w.Write(compiledSql,
                                               string.Format("{0}\\{1}_AllVersionsUpgrade.sql", compileScriptPath,
                                                             databaseName),
                                               string.Format("{0}\\{1}_AllVersionsRollback.sql", compileScriptPath,
                                                             databaseName)));


            _compiler.CompileAllVersions(_database, compileScriptPath);
        }
    }
}