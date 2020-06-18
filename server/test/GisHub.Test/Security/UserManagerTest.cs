using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Beginor.GisHub.Data.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace Beginor.GisHub.Test.Security {

    [TestFixture]
    public class UserManagerTest : BaseTest<UserManager<AppUser>> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public void _02_CanQueryAllUsers() {
            var users = Target.Users.ToList();
            Assert.IsNotNull(users);
        }

        [Test]
        public async Task _03_CanCreateAdminAsync() {
            var exists = await Target.FindByNameAsync("admin");
            if (exists == null) {
                // create admin user;
                var user = new AppUser {
                    UserName = "admin",
                    Email = "admin@local.com",
                    EmailConfirmed = true,
                    PhoneNumber = "02088888888",
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false
                };
                await Target.CreateAsync(user);
                Assert.IsNotNull(user.Id);
                // add password;
                var result = await Target.AddPasswordAsync(user, "1qaz@WSX");
                Assert.IsTrue(result.Succeeded);
                // add to administrators;
                var roleName = "administrators";
                if (!await Target.IsInRoleAsync(user, roleName)) {
                    var result2 = await Target.AddToRoleAsync(user, roleName);
                    Assert.IsTrue(result2.Succeeded);
                }
            }
        }

    }

}
