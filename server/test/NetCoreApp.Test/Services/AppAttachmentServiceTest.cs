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
        [TestCase("", "")]
        [TestCase("000", "")]
        [TestCase("", "xml")]
        [TestCase("000", "xml")]
        public async Task _02_CanSearch(string userId, string contentType) {
            var model = new AppAttachmentSearchModel {
                Skip = 0,
                Take = 1,
                CreatorId = userId,
                ContentType = contentType
            };
            var result = await Target.Search(model);
            Assert.NotNull(result);
        }

        [Test]
        [TestCase("0000")]
        public async Task _03_CanGetByUser(string userId) {
            var result = await Target.GetByUser(userId);
            Assert.NotNull(result);
            if (result.Count > 0) {
                foreach (var model in result) {
                    Assert.AreEqual(userId, model.CreatorId);
                }
            }
        }

    }

}
