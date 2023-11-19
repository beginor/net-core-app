using NUnit.Framework;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class AppPrivilegeRepositoryTest : BaseTest<IAppPrivilegeRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
