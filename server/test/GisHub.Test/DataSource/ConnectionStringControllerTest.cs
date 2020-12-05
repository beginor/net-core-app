using Beginor.GisHub.DataServices.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Api {

    [TestFixture]
    public class ConnectionStringControllerTest : BaseTest<ConnectionStringController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
