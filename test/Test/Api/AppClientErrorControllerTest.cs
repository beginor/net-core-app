using NUnit.Framework;
using Beginor.NetCoreApp.Api.Controllers;

namespace Beginor.NetCoreApp.Test.Api;

[TestFixture]
public class AppClientErrorControllerTest : BaseTest<AppClientErrorController> {

    [Test]
    public void _01_CanResolveTarget() {
         Assert.That(Target, Is.Not.Null);
    }

}
