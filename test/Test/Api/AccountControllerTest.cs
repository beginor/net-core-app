using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.NetCoreApp.Api.Controllers;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Test.Api;

[TestFixture]
public class AccountControllerTest : BaseTest<AccountController> {

    // controllers are registered as transient, keep the resolved instance;
    private AccountController controller;
    private UserManager<AppUserEntity> userMgr;
    private string originalSurname;
    private string originalGivenName;

    [SetUp]
    public async Task SetUp() {
        userMgr = ServiceProvider.GetRequiredService<UserManager<AppUserEntity>>();
        controller = Target;
        var httpContext = new DefaultHttpContext();
        // GetInfo reads Request.Query, a bare DefaultHttpContext has no request feature;
        httpContext.Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
        httpContext.User = CreateTestPrincipal();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        var admin = await userMgr.FindByNameAsync("admin");
        originalSurname = admin?.Surname;
        originalGivenName = admin?.GivenName;
    }

    [TearDown]
    public async Task TearDown() {
        // restore admin profile so the shared database stays unchanged;
        var admin = await userMgr.FindByNameAsync("admin");
        if (admin != null) {
            admin.Surname = originalSurname;
            admin.GivenName = originalGivenName;
            await userMgr.UpdateAsync(admin);
        }
    }

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(controller, Is.Not.Null);
    }

    [Test]
    public async Task _02_GetInfo_ShouldReturnProfileClaims() {
        var admin = await userMgr.FindByNameAsync("admin");
        Assert.That(admin, Is.Not.Null);
        admin.Surname = "测试";
        admin.GivenName = "管理员";
        Assert.That((await userMgr.UpdateAsync(admin)).Succeeded, Is.True);

        var info = (await controller.GetInfo()).Value;
        Assert.That(info, Is.Not.Null);
        Assert.That(info.UserName, Is.EqualTo("admin"));
        // profile claims are cached instead of written into the token,
        // account info must still carry them for the client;
        Assert.That(info.Surname, Is.EqualTo("测试"));
        Assert.That(info.GivenName, Is.EqualTo("管理员"));
    }

}
