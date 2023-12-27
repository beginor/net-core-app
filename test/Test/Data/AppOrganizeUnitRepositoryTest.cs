using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

/// <summary>组织单元仓储测试</summary>
[TestFixture]
public class AppOrganizeUnitRepositoryTest : BaseTest<IAppOrganizeUnitRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppOrganizeUnitSearchModel {
            OrganizeUnitId = 1L
        };
        var result = await Target.SearchAsync(searchModel, CreateTestPrincipal());
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task _03_CanQueryPath() {
        var units = await Target.QueryPathAsync(1L);
        Assert.That(units, Is.Not.Empty);
        Console.WriteLine(units.ToJson(GetTestJsonOption()));
    }

    [Test]
    public async Task _03_CanCheckUserUnit() {
        var userUnitId = 1L;
        var canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678508063020798L);
        Assert.That(canView);
        userUnitId = 1701678508063020798L;
        canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678508063020798L);
        Assert.That(canView);
        canView = await Target.CanViewOrganizeUnitAsync(userUnitId, 1701678232194020784L);
        Assert.That(canView, Is.False);
    }

    [Test]
    public async Task _04_CanGetById() {
        var id = 1701678508063020798L;
        var model = await Target.GetByIdAsync(id, CreateTestPrincipal());
        Assert.That(model.Id, Is.EqualTo(id.ToString()));
        Console.WriteLine(model.ToJson(GetTestJsonOption()));
    }

}
