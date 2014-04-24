using DBTM.Application.Factories;
using DBTM.Application.SQL;
using NUnit.Framework;

namespace Tests.Application
{
    [TestFixture]
    public class DbCommandFactoryTests
    {

        [Test]
        public void CreateReturnsNewDbCommand()
        {
            IDbCommandFactory commandFactory = new DbCommandFactory();

            var sql = "some sql";
            var command = commandFactory.Create(sql);

            Assert.AreEqual(sql, command.CommandText);
        }
    }
}