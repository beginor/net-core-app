using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Security {

    [TestClass]
    public class RoleManagerTest : BaseTest<RoleManager<ApplicationRole>> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
