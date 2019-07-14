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
    public class AppNavItemRepositoryTest : BaseTest<IAppNavItemRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanDoSoftDelete() {
            var entity = new AppNavItem {
                Title = "Test Item",
                Tooltip = "Test Nav item",
                Icon = null,
                Url = "/test",
                ParentId = 0,
                Sequence = 0,
                Creator = new AppUser {
                    Id = "1546069735288020001"
                },
                CreatedAt = DateTime.Now,
                Updater = new AppUser {
                    Id = "1546069735288020001",
                }
            };
            await Target.SaveAsync(entity);
            Assert.Greater(entity.Id, 0);
            await Target.DeleteAsync(entity.Id);
            entity = await Target.GetByIdAsync(entity.Id);
            Assert.IsNull(entity);
        }

    }

}
