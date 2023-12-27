using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

[TestFixture]
public class AppNavItemRepositoryTest : BaseTest<IAppNavItemRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanDoSoftDelete() {
        var entity = new AppNavItemModel {
            Title = "Test Item",
            Tooltip = "Test Nav item",
            Icon = null,
            Url = "/test",
            ParentId = "0",
            Sequence = 0
        };
        await Target.SaveAsync(entity);
        Assert.That(entity.Id, Is.Not.Empty);
        await Target.DeleteAsync(long.Parse(entity.Id));
        entity = await Target.GetByIdAsync(long.Parse(entity.Id));
        Assert.That(entity, Is.Null);
    }

    [Test]
    public async Task _03_CanQueryEmpty() {
        var sql = "select * from public.app_nav_items where 1 = 0";
        using var session = ServiceProvider.GetService<NHibernate.ISession>();
        var conn = session.Connection;
        var navItems = await conn.QueryAsync<AppNavItem>(sql);
        Assert.That(navItems, Is.Not.Null);
        Assert.That(navItems, Is.Empty);
    }

}
