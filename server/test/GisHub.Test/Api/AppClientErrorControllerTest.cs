using NUnit.Framework;
using Beginor.GisHub.Api.Controllers;

namespace Beginor.GisHub.Test.Api;

[TestFixture]
public class AppClientErrorControllerTest : BaseTest<AppClientErrorController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
