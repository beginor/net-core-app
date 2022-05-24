using Beginor.GisHub.Common;
using NUnit.Framework;

namespace Beginor.GisHub.Test.Data; 

[TestFixture]
public class ResourceRolesFilterProviderTest : BaseTest<IRolesFilterProvider> {

    [Test]
    public void _00_CanResolveTarget() {
        var target = Target;
        Assert.IsNotNull(target);
    }

}
