using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Test.Security;

[TestFixture]
public class RoleManagerTest : BaseTest<RoleManager<AppRole>> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public void _02_CanQueryAllRoles() {
        var roles = Target.Roles.ToList();
        Assert.That(roles, Is.Not.Null);
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
            Assert.That(role.Id, Is.Not.Empty);
            // create privileges;
            var repo = ServiceProvider.GetService<IAppPrivilegeRepository>();
            var privileges = await repo.GetAllAsync();
            foreach (var priv in privileges) {
                var claim = new Claim(Consts.PrivilegeClaimType, priv.Name);
                await Target.AddClaimAsync(role, claim);
            }
            var claims = await Target.GetClaimsAsync(role);
            Assert.That(privileges.Count, Is.EqualTo(claims.Count));
        }
    }

}
