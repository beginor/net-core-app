using Beginor.GisHub.DataServices.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataSources {

    [TestFixture]
    public class ConnectionControllerTest : BaseTest<ConnectionController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
