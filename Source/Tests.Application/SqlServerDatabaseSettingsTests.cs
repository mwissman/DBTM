using DBTM.Application;
using NUnit.Framework;
using Rhino.Mocks;

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

        [Test]
        public void ConnectionStringBuilderUsesAuthenticationInConnectionString()
        {
            string dbName = "name";
            string server = "server";
            string datafilePath = "blah";
            var auth = MockRepository.GenerateMock<IAuthenticantion>();
            auth.Stub(a => a.ToConnectionStringFragment()).Return("--AuthFragement--;");

            var expectedAdminConnectionString = string.Format("Data Source={0};Initial Catalog=Master;--AuthFragement--;Pooling=false;",server);

            var expectedConnectionString = string.Format("Data Source={0};Initial Catalog={1};--AuthFragement--;Pooling=false;", server, dbName);

            ISqlServerDatabaseSettings settings = new SqlServerDatabaseSettings(dbName,
                                                                                server,
                                                                                datafilePath,
                                                                                string.Empty,
                                                                                auth);

            Assert.AreEqual(expectedAdminConnectionString, settings.AdminConnectionString);
            Assert.AreEqual(expectedConnectionString, settings.ConnectionString);

        }
    }

    [TestFixture]
    public class AuthenticationTests
    {
        [Test]
        public void UsernameAndPasswordToConnectionStringFragment()
        {
            UsernamePassword auth=new UsernamePassword("user","passowrd");
            Assert.AreEqual("User Id=user;Password=passowrd;", auth.ToConnectionStringFragment());
        }

        [Test]
        public void UserAuthToConnectionStringFragment()
        {
            WindowsAuth auth = new WindowsAuth();
            Assert.AreEqual("Trusted_Connection=Yes;", auth.ToConnectionStringFragment());
        }
    }
   
}