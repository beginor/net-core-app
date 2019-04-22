using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Test.Security {

    [TestFixture]
    public class UserManagerTest : BaseTest<UserManager<AppUser>> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanQueryAllUsers() {
            var users = await Target.Users.ToListAsync();
            Assert.IsNotNull(users);
        }

    }

}
