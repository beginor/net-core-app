using NUnit.Framework;
using Beginor.GisHub.Api.Controllers;

namespace Beginor.GisHub.Test.Api;

[TestFixture]
public class AppAttachmentsControllerTest : BaseTest<AppAttachmentController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
