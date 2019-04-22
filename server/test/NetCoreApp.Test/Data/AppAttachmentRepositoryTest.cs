using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Repositories;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        [TestCase("user01")]
        [TestCase("user02")]
        public async Task _02_CanGetByUser(string userId) {
            var data = await Target.GetByUser(userId);
            Assert.IsNotNull(data);
        }

    }
}
