using System;
using System.Collections.Generic;
using DBTM.Application;
using DBTM.Application.SQL;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Application
{
    [TestFixture]
    public class SqlServerDatabaseTests
    {
        [Test]
        public void DisconnectAllUsers_DropExistingSchema_CreateEmptySchema_GrantAccessToSpecifiedUser()
        {
            string databaseFilePath = @"C:\blah\foo";
            string databaseName = "databaseName";
            string server = "server";

            string disconnectscript = "disconnectScript";
            string dropscript = "dropScript";
            string createscript = "createScript";
            string connectionString = "some connection string";

            MockRepository mockRepository = new MockRepository();

            ISqlServerDatabaseSettings settings = mockRepository.Stub<ISqlServerDatabaseSettings>();
            ISqlRunner sqlRunner = mockRepository.StrictMock<ISqlRunner>();
            ISqlScriptRepository sqlScriptRepository = mockRepository.DynamicMock<ISqlScriptRepository>();
            settings.Stub(s => s.DatabaseFilePath).Return(databaseFilePath);
            settings.Stub(s => s.DatabaseName).Return(databaseName);
            settings.Stub(s => s.Server).Return(server);
            settings.Stub(s => s.AdminConnectionString).Return(connectionString);

            sqlScriptRepository.Expect(s => s.LoadDisconnectUser(databaseName)).Return(disconnectscript);
            sqlScriptRepository.Expect(s => s.LoadDropSchema(databaseName)).Return(dropscript);
            sqlScriptRepository.Expect(s => s.LoadCreateSchema(databaseName, databaseFilePath)).Return(createscript);


            sqlRunner.Expect(s => s.RunAdminScripts(Arg<IList<string>>.Matches(
                                                        scripts =>
                                                        scripts[0] == disconnectscript &&
                                                        scripts[1] == dropscript &&
                                                        scripts[2] == createscript
                                                        ),
                                                    Arg<string>.Is.Equal(connectionString)));


            mockRepository.ReplayAll();


            ISqlServerDatabase sqlServerDatabase = new SqlServerDatabase(sqlRunner, sqlScriptRepository);
            sqlServerDatabase.Initialize(settings);

            mockRepository.VerifyAll();
        }
    }
}