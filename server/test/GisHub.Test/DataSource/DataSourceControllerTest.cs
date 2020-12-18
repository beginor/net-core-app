using Beginor.GisHub.DataServices.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataSource {

    [TestFixture]
    public class DataSourceControllerTest : BaseTest<DataSourceController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}