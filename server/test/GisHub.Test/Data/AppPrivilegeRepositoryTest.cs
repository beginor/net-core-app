using NUnit.Framework;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Test.Data;

[TestFixture]
public class AppPrivilegeRepositoryTest : BaseTest<IAppPrivilegeRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
