using DBTM.Cmd.Arguments;
using NUnit.Framework;

namespace Tests.Cmd
{
    [TestFixture]
    public class RunDirectoryOfSqlArgumentsTests
    {
        [Test]
        public void HasRequiredNeedsAFilePath()
        {
            string scriptPath = @"c:\work\database";

            var args = new[] { "-scriptPath=" + scriptPath };

            var arguments = new RunDirectoryOfSqlArguments(args);

            Assert.AreEqual(scriptPath, arguments.ScriptDirectoryPath);
            Assert.IsTrue(arguments.HasRequiredArguments);

        }

        [Test]
        public void HasRequiredFailsWhenNoPathProvided()
        {
            var args = new[] { "-asfsdaf=sdafsf"  };

            var arguments = new RunDirectoryOfSqlArguments(args);

            Assert.IsFalse(arguments.HasRequiredArguments);

        }

    }
}