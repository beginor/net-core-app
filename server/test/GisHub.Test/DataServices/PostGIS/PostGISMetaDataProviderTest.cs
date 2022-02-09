using NUnit.Framework;
using Beginor.GisHub.DataServices.PostGIS;

namespace Beginor.GisHub.Test.DataServices.PostGIS {

    [TestFixture]
    public class PostGISMetaDataProviderTest : BaseTest<PostGISMetaDataProvider> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
