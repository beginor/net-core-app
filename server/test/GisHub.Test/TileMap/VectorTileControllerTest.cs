using Beginor.GisHub.TileMap.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.TileMap {

    [TestFixture]
    public class VectorTileControllerTest : BaseTest<VectorTileController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
