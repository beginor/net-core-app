
using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Api {

    [TestFixture]
    public class SpaFailbackTest {

        [Test]
        [TestCase("/web/home")]
        [TestCase("/web/about")]
        [TestCase("/web/admin/users")]
        [TestCase("/web/assets/icon")]
        public void CanMatchUrl(string url) {
            var shouldIgnore = url.ToLowerInvariant().StartsWith("/web/assets/");
            var regex = new Regex("/web/(?!assets/).*");
            var isMatch = regex.IsMatch(url);
            Assert.AreEqual(shouldIgnore, !isMatch);
            var match = regex.Match(url);
            Console.WriteLine(match);
        }

    }

}
