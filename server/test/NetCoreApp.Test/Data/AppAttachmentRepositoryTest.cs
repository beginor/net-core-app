using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Data {

    [TestClass]
    public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [TestMethod]
        public async Task _02_CanGetByUser() {
            var data = await Target.GetByUser("test");
            Assert.IsNotNull(data);
        }

    }
}
