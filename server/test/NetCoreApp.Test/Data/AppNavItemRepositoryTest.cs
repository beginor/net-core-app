using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;
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
            var entity = new AppNavItemModel {
                Title = "Test Item",
                Tooltip = "Test Nav item",
                Icon = null,
                Url = "/test",
                ParentId = "0",
                Sequence = 0
            };
            await Target.SaveAsync(entity);
            Assert.IsNotEmpty(entity.Id);
            await Target.DeleteAsync(long.Parse(entity.Id));
            entity = await Target.GetByIdAsync(long.Parse(entity.Id));
            Assert.IsNull(entity);
        }

    }

}
