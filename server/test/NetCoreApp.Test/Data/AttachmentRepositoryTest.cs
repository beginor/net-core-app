using Beginor.NetCoreApp.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Data {

    [TestClass]
    public class AttachmentRepositoryTest : BaseTest<IAttachmentRepository> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }
}
