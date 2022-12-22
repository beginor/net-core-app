using NUnit.Framework;
using Beginor.GisHub.Api.Controllers;

namespace Beginor.GisHub.Test.Api;

[TestFixture]
public class AppAuditLogsControllerTest : BaseTest<AppAuditLogController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
