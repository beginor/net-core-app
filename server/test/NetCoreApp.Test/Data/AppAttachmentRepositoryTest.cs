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
        public async Task _02_CanGetByUser() {
            var data = await Target.GetByUser("test");
            Assert.IsNotNull(data);
        }

    }
}
