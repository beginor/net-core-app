using NUnit.Framework;
using Beginor.NetCoreApp.Api.Controllers;

namespace Beginor.NetCoreApp.Test.Api;

[TestFixture]
public class AppAttachmentsControllerTest : BaseTest<AppAttachmentController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

}
