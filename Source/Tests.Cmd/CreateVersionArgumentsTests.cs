using DBTM.Cmd.Arguments;
using NUnit.Framework;

namespace Tests.Cmd
{
    [TestFixture]
    public class CreateVersionArgumentsTests
    {
        [Test]
        public void ParsesArguments()
        {
            string databaseFilePath = @"c:\work\database\databasefile.dbschema";

            var args = new[] {"-databaseFilePath=" + databaseFilePath};

            CreateVersionArguments arguments = new CreateVersionArguments(args);

            Assert.IsTrue(arguments.HasRequiredArguments);
            Assert.AreEqual(databaseFilePath, arguments.DatabaseSchemaFilePath);
        }


        [Test]
        public void HasRequiredIsFalseIfDatabaseFile()
        {
            string[] args = new[] { "-somethingelse=value" };

            CompileScriptsArguments arguments = new CompileScriptsArguments(args);

            Assert.IsFalse(arguments.HasRequiredArguments);
        }

    }
}