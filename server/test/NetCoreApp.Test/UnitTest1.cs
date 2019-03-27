using NUnit.Framework;

namespace Beginor.NetCoreApp.Test {

    public class Tests {

        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            Assert.Pass();
        }

        [Test]
        public void Test2() {
            System.Console.WriteLine("Test2");
            Assert.Fail();
        }

    }

}
