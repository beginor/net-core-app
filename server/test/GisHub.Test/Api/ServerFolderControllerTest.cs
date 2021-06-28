using Beginor.GisHub.Api.Controllers;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Api {

    [TestFixture]
    public class ServerFolderControllerTest : BaseTest<ServerFolderController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
