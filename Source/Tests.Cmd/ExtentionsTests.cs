using System;
using DBTM.Cmd;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Cmd
{
    [TestFixture]
    public class ExtentionsTests
    {
        [TestCase("name","value")]
        [TestCase("NAME","value")]
        [TestCase("nAmE","value")]
        public void GetValueReturnsValueOfArgumentParameterAndIsCaseInsensitive(string name, string value)
        {
            string[] args = {
                                "-a=b",
                                "-" + name + "=" + value,
                                "-blah=foo"
                            };

            var result = args.GetArgumentValue("name");

            Assert.AreEqual("value",result);
        }

        [TestCase("name")]
        [TestCase("NAME")]
        [TestCase("nAmE")]
        public void HasValueReturnsTrueIfArgArrayContainsFlagCaseInsensitive(string name)
        {
            string[] args = {
                                "-a=b",
                                "-" + name + "= value",
                                "-blah=foo"
                            };

            var result = args.HasArgumentValue(name);

            Assert.IsTrue(result);

        }

    }
}