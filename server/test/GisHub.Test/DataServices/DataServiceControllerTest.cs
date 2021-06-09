using Beginor.GisHub.DataServices.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataServices {

    [TestFixture]
    public class DataServiceControllerTest : BaseTest<DataServiceController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
