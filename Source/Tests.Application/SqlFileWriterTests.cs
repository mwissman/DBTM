using DBTM.Application;
using DBTM.Domain.Entities;
using DBTM.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application
{
    [TestFixture]
    public class SqlFileWriterTests
    {
        private SqlFileWriter _sqlWriter;
        private IStreamWriter _streamWriter;
       

        [SetUp]
        public void Setup()
        {
            _streamWriter = MockRepository.GenerateMock<IStreamWriter>();

            _sqlWriter = new SqlFileWriter(_streamWriter);
        }

        [TearDown]
        public void TearDown()
        {
            _streamWriter.VerifyAllExpectations();
        }

        [Test]
        public void WriteSavesUpgradeToUpgradePathAndRollbackToRollbackPath()
        {
            string rollbackPath = "rollback";
            string upgradePath = "upgrade";

            string upgradesql = "sql";
            string rollbacksql = "rollbackSql";
            
            var compiledSql = new CompiledSql(upgradesql, rollbacksql);

            _streamWriter.Expect(sw => sw.Write(upgradePath, upgradesql));
            _streamWriter.Expect(sw => sw.Write(rollbackPath, rollbacksql));
 
            _sqlWriter.Write(compiledSql, upgradePath, rollbackPath);
        }

       
    }
}