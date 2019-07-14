using System.Threading.Tasks;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Services {

    [TestFixture]
    public class AppNavItemServiceTest : BaseTest<IAppNavItemService> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanSearch() {
            var model = new AppNavItemSearchModel {
                Skip = 0,
                Take = 10
            };
            var result = await Target.SearchAsync(model);
            Assert.IsNotNull(result);
            Assert.AreEqual(model.Skip, result.Skip);
            Assert.AreEqual(model.Take, result.Take);
            Assert.IsNotNull(result.Data);
            Assert.GreaterOrEqual(model.Take, result.Data.Count);
        }

        [Test]
        public async Task _03_CanDoSoftDelete() {
            var model = new AppNavItemModel {
                ParentId = "0",
                Title = "test model",
                Tooltip = "",
                Icon = "",
                Url = "",
                Sequence = 0
            };
            await Target.CreateAsync(model, "beginor@qq.com");
            Assert.IsNotEmpty(model.Id);
            var modelInDb = await Target.GetByIdAsync(model.Id);
            Assert.IsNotNull(modelInDb);
            Assert.AreEqual(model.Id, modelInDb.Id);
            modelInDb.Tooltip = "Test tooltip";
            await Target.UpdateAsync(model.Id, model, "beginor@qq.com");
            await Target.DeleteAsync(model.Id);
            modelInDb = await Target.GetByIdAsync(model.Id);
            Assert.IsNull(modelInDb);
        }

    }

}
