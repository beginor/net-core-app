using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Test.Data; 

[TestFixture]
public class AppNavItemRepositoryTest : BaseTest<IAppNavItemRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
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
        Assert.IsNotEmpty(entity.Id);
        await Target.DeleteAsync(long.Parse(entity.Id));
        entity = await Target.GetByIdAsync(long.Parse(entity.Id));
        Assert.IsNull(entity);
    }

}