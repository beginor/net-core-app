using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Security {

    [TestFixture]
    public class UserManagerTest : BaseTest<UserManager<ApplicationUser>> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.NotNull(Target);
        }

    }

}
