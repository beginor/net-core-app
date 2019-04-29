using System.Threading.Tasks;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Services {

    [TestFixture]
    public class AppAttachmentServiceTest : BaseTest<IAppAttachmentService> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public async Task _02_CanSearch() {
            var model = new AppAttachmentSearchModel {
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

    }

}
