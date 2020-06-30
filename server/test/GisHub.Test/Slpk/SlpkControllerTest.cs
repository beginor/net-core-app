using Beginor.GisHub.Api.Controllers;
using Beginor.GisHub.Slpk.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Slpk {

    [TestFixture]
    public class SlpkControllerTest : BaseTest<SlpkController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
