using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Beginor.GisHub.Data.Entities;
using NUnit.Framework;
using NHibernate.Linq;

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