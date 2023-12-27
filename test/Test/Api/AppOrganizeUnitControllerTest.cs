using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.NetCoreApp.Api.Controllers;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Api;

[TestFixture]
public class AppOrganizeUnitControllerTest : BaseTest<AppOrganizeUnitController> {

    [Test]
    public void _01_CanResolveTarget() {
         Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanSearchOrganizeUnit() {
        var model = new AppOrganizeUnitSearchModel {
            Take = 10,
            Skip = 0
        };
        var result = await Target.Search(model);
        Assert.That(result, Is.Not.Null);
    }

}
