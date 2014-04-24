using DBTM.UI;
using NUnit.Framework;

namespace Tests.UI
{
    [TestFixture]
    public class ArgumentsProviderTests
    {
        [Test]
        public void WhenFirstArgumentIsAFilePathHasFileIsTru()
        {
            string file = @"c:\foo\blah.xml";
            string[] args = new[] {file};
            ArgumentsProvider argumentsProvider = new ArgumentsProvider(args);

            var result=argumentsProvider.HasFile;

            Assert.IsTrue(result);
            Assert.AreEqual(argumentsProvider.FilePath, file);
        }

        [Test]
        public void WhenFirstArgumentIsEmptyHasFileIsFalse()
        {
            string[] args = new string[] {  };
            ArgumentsProvider argumentsProvider = new ArgumentsProvider(args);

            var result = argumentsProvider.HasFile;

            Assert.IsFalse(result);
            Assert.IsNull(argumentsProvider.FilePath);
        }


        [Test]
        public void WhenFirstArgumentStartsWithADashPathHasFileIsFalse()
        {
            string[] args = new string[] { "-asdfhasdjf=asdfj" };
            ArgumentsProvider argumentsProvider = new ArgumentsProvider(args);

            var result = argumentsProvider.HasFile;

            Assert.IsFalse(result);
            Assert.IsNull(argumentsProvider.FilePath);
        }
    }
}