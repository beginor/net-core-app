using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Security {

    [TestFixture]
    public class RoleManagerTest : BaseTest<RoleManager<ApplicationRole>> {

        [SetUp]
        public void SetUp() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void _01_CanResolveTarget() {
            Assert.NotNull(Target);
        }

    }

}
