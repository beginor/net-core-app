using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

/// <summary>组织单元仓储测试</summary>
[TestFixture]
public class AppOrganizeUnitRepositoryTest : BaseTest<IAppOrganizeUnitRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppOrganizeUnitSearchModel {
            OrganizeUnitId = 1L
        };
        var result = await Target.SearchAsync(searchModel, CreateTestPrincipal());
        Assert.GreaterOrEqual(result.Total, 0);
    }

    [Test]
    public async Task _03_CanQueryPath() {
        var units = await Target.QueryPathAsync(1L);
        Assert.IsNotEmpty(units);
        Console.WriteLine(units.ToJson(GetTestJsonOption()));
    }

    [Test]
    public async Task _03_CanCheckUserUnit() {
        var userUnitId = 1L;
        var canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678508063020798L);
        Assert.IsTrue(canView);
        userUnitId = 1701678508063020798L;
        canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678508063020798L);
        Assert.IsTrue(canView);
        canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678232194020784L);
        Assert.IsFalse(canView);
    }

    [Test]
    public async Task _04_CanGetById() {
        var id = 1701678508063020798L;
        var model = await Target.GetByIdAsync(id, CreateTestPrincipal());
        Assert.AreEqual(model.Id, id.ToString());
        Console.WriteLine(model.ToJson(GetTestJsonOption()));
    }

    private ClaimsPrincipal CreateTestPrincipal() {
        var identity = new ClaimsIdentity(new [] {
            new Claim(ClaimTypes.NameIdentifier, "1578371512959020099"),
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(Consts.OrganizeUnitIdClaimType, "1")
        }, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

}
