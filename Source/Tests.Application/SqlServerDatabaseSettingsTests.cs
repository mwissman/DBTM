using DBTM.Application;
using NUnit.Framework;

namespace Tests.Application
{
    [TestFixture]
    public class SqlServerDatabaseSettingsTests
    {
        [Test]
        public void ConnectionStringBuildsConnectionStringFromOtherSettings()
        {
            string dbName = "name";
            string server = "server";
            string user = "user";
            string datafilePath = "blah";
            string password = "pwd";

            var expectedAdminConnectionString = string.Format("Data Source={0};Initial Catalog=Master;User Id={1};Password={2};Pooling=false;",
                                         server,
                                         user,
                                         password);

            var expectedConnectionString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Pooling=false;",
                                         server,
                                         dbName,
                                         user,
                                         password);

            ISqlServerDatabaseSettings settings = new SqlServerDatabaseSettings(dbName,
                                                                                server,
                                                                                user,
                                                                                datafilePath,
                                                                                password,
                                                                                string.Empty);

            Assert.AreEqual(expectedAdminConnectionString, settings.AdminConnectionString);
            Assert.AreEqual(expectedConnectionString, settings.ConnectionString);
        }
    }
}