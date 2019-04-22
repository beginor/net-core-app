using Beginor.NetCoreApp.Services;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Services {

    [TestFixture]
    public class AppAttachmentServiceTest : BaseTest<IAppAttachmentService> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
