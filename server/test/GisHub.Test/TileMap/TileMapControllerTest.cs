using Beginor.GisHub.TileMap.Api;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Api;

[TestFixture]
public class TileMapControllerTest : BaseTest<TileMapController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
