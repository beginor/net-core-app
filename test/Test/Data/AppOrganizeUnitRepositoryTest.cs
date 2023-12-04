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
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppOrganizeUnitSearchModel {
            OrganizeUnitId = 1L
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.GreaterOrEqual(result.Total, 0);
        Assert.GreaterOrEqual(result.Take, result.Data.Count);
    }

    [Test]
    public async Task _03_CanQueryPath() {
        var units = await Target.QueryPathAsync(1701678508063020798L);
        Assert.IsNotEmpty(units);
        Console.WriteLine(units.ToJson());
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

}
