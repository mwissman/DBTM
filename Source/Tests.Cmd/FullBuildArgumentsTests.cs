using DBTM.Cmd;
using DBTM.Cmd.Arguments;
using NUnit.Framework;

namespace Tests.Cmd
{
    [TestFixture]
    public class FullBuildArgumentsTests
    {
        [Test]
        public void ParsesArguments()
        {
            string databaseName = "somedatabase";
            string server = "someserver";
            string userName = "someone";
            string dataFilePath = @"c:\redbox\database\datafile.mdf";
            string databaseFilePath = @"c:\redbox\database\databasefile.dbschema";
            string password = "password";
            string crossDatabaseNamePrefix = "TestBranch_";

            string[] args = new[]
                                {
                                    "-databaseName=" + databaseName,
                                    "-server=" + server,
                                    "-userName=" + userName,
                                    "-dataFilePath=" + dataFilePath,
                                    "-databaseFilePath=" + databaseFilePath,
                                    "-password=" + password,
                                    "-crossDatabaseNamePrefix=" + crossDatabaseNamePrefix
                                };

            FullBuildArguments fullBuildArguments = new FullBuildArguments(args);

            Assert.IsTrue(fullBuildArguments.HasRequiredArguments);
            Assert.AreEqual(databaseName, fullBuildArguments.DatabaseName);
            Assert.AreEqual(server, fullBuildArguments.Server);
            Assert.AreEqual(userName, fullBuildArguments.UserName);
            Assert.AreEqual(dataFilePath, fullBuildArguments.DataFilePath);
            Assert.AreEqual(password, fullBuildArguments.Password);
            Assert.AreEqual(crossDatabaseNamePrefix, fullBuildArguments.CrossDatabaseNamePrefix);
        }

        [Test]
        public void OptionalArgumentDefaultsAreCorrectParsesArguments()
        {
            string databaseName = "somedatabase";
            string server = "someserver";
            string userName = "someone";
            string dataFilePath = @"c:\redbox\database\datafile.mdf";
            string databaseFilePath = @"c:\redbox\database\databasefile.dbschema";
            string password = "password";
            
            string[] args = new[]
                                {
                                    "-databaseName=" + databaseName,
                                    "-server=" + server,
                                    "-userName=" + userName,
                                    "-dataFilePath=" + dataFilePath,
                                    "-databaseFilePath=" + databaseFilePath,
                                    "-password=" + password,
                                };

            FullBuildArguments fullBuildArguments = new FullBuildArguments(args);

            Assert.IsTrue(fullBuildArguments.HasRequiredArguments);
            Assert.AreEqual(databaseName, fullBuildArguments.DatabaseName);
            Assert.AreEqual(server, fullBuildArguments.Server);
            Assert.AreEqual(userName, fullBuildArguments.UserName);
            Assert.AreEqual(dataFilePath, fullBuildArguments.DataFilePath);
            Assert.AreEqual(password, fullBuildArguments.Password);
            Assert.AreEqual(string.Empty, fullBuildArguments.CrossDatabaseNamePrefix);
        }
    }
}