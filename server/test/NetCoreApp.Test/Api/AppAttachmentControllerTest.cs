using Beginor.NetCoreApp.Api.Controllers;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Api {

    [TestFixture]
    public class AppAttachmentsControllerTest : BaseTest<AppAttachmentController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
