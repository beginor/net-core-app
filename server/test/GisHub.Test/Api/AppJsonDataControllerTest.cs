using NUnit.Framework;
using Beginor.GisHub.Api.Controllers;

namespace Beginor.GisHub.Test.Api;

[TestFixture]
public class AppJsonDataControllerTest : BaseTest<AppJsonDataController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
