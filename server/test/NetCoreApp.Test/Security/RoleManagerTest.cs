using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
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
        public void _02_CanQueryAllRoles() {
            var roles = Target.Roles.ToList();
            Assert.IsNotNull(roles);
        }

        [Test]
        public async Task _03_CanCreateAdministrators() {
            var exists = await Target.RoleExistsAsync("administrators");
            if (!exists) {
                // create administrators role;
                var role = new AppRole {
                    Name = "administrators",
                    Description = "系统管理员"
                };
                await Target.CreateAsync(role);
                Assert.IsNotEmpty(role.Id);
                // create privileges;
                var repo = ServiceProvider.GetService<IAppPrivilegeRepository>();
                var privileges = await repo.GetAllAsync();
                foreach (var priv in privileges) {
                    var claim = new Claim(Consts.PrivilegeClaimType, priv.Name);
                    await Target.AddClaimAsync(role, claim);
                }
                var claims = await Target.GetClaimsAsync(role);
                Assert.AreEqual(privileges.Count, claims.Count);
            }
        }

    }

}
