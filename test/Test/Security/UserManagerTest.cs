using System.Linq;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Test.Security;

[TestFixture]
public class UserManagerTest : BaseTest<UserManager<AppUser>> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public void _02_CanQueryAllUsers() {
        var users = Target.Users.ToList();
        Assert.That(users, Is.Not.Null);
    }

}
