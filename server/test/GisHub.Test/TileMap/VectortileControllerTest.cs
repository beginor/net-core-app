using Beginor.GisHub.TileMap.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.VectorTile {

    [TestFixture]
    public class VectortileControllerTest : BaseTest<VectortileController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}
