using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppPrivilegeRepositoryTest : BaseTest<IAppPrivilegeRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        [TestCase("app_roles.read")]
        public async Task _02_CanQueryExistAsync(string name) {
            var exists = await Target.ExistsAsync(name);
            Assert.True(exists);
        }

    }

}
