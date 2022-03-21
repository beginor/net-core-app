using Beginor.GisHub.Api.Controllers;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Api; 

[TestFixture]
public class RolesControllerTest : BaseTest<RolesController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}