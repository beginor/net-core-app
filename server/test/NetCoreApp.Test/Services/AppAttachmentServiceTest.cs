using Beginor.NetCoreApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test.Services {

    [TestClass]
    public class AppAttachmentServiceTest : BaseTest<IAppAttachmentService> {

        [TestMethod]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
