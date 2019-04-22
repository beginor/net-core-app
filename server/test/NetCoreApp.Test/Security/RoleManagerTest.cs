using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Test.Security {

    [TestClass]
    public class RoleManagerTest : BaseTest<RoleManager<AppRole>> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [TestMethod]
        public async Task _02_CanQueryAllRoles() {
            var roles = await Target.Roles.ToListAsync();
            Assert.IsNotNull(roles);
        }

    }

}
