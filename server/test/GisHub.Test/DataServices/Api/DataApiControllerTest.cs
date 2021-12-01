using Beginor.GisHub.DataServices.Api;
using Beginor.GisHub.DynamicSql.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataServices.Api {

    [TestFixture]
    public class DataApiControllerTest : BaseTest<DataApiController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}