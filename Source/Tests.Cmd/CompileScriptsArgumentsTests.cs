using DBTM.Cmd.Arguments;
using NUnit.Framework;

namespace Tests.Cmd
{
    [TestFixture]
    public class CompileScriptsArgumentsTests
    {
        [Test]
        public void ParsesArguments()
        {
            string compileDirectory = "c:\\some\folder";     
            string databaseFilePath = @"c:\redbox\database\databasefile.dbschema";

            string[] args = new[]{
                                    "-CompiledScriptDirectory=" + compileDirectory,
                                    "-databaseFilePath=" + databaseFilePath,
                                };

            CompileScriptsArguments arguments = new CompileScriptsArguments(args);

            Assert.IsTrue(arguments.HasRequiredArguments);
            Assert.AreEqual(compileDirectory, arguments.CompiledScriptDirectory);
            Assert.AreEqual(databaseFilePath, arguments.DatabaseSchemaFilePath);
        }

        [Test]
        public void HasRequiredIsFalseIfMissingCompiledScriptDirectory()
        {
            string[] args = new[] {"-databaseFilePath=value"};

            CompileScriptsArguments arguments = new CompileScriptsArguments(args);

            Assert.IsFalse(arguments.HasRequiredArguments);
        }

        [Test]
        public void HasRequiredIsFalseIfDatabaseFile()
        {
            string[] args = new[] { "-CompiledScriptDirectory=value" };

            CompileScriptsArguments arguments = new CompileScriptsArguments(args);

            Assert.IsFalse(arguments.HasRequiredArguments);
        }

    }
}