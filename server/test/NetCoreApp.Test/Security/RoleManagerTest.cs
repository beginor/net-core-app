using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Test.Security {

    [TestFixture]
    public class RoleManagerTest : BaseTest<RoleManager<AppRole>> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        [Ignore("Do not test UserManager")]
        public async Task _02_CanQueryAllRoles() {
            var roles = await Target.Roles.ToListAsync();
            Assert.IsNotNull(roles);
        }

    }

}
