using System.Linq;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;

namespace Beginor.GisHub.Test.Security;

[TestFixture]
public class UserManagerTest : BaseTest<UserManager<AppUser>> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public void _02_CanQueryAllUsers() {
        var users = Target.Users.ToList();
        Assert.IsNotNull(users);
    }

}
