using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

/// <summary>json 数据仓储测试</summary>
[TestFixture]
public class AppJsonDataRepositoryTest : BaseTest<IAppJsonDataRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppJsonDataSearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Take, Is.GreaterThanOrEqualTo(result.Data.Count));
    }

    [Test]
    public async Task _03_CanSaveAndDeleteAsync() {
        var val = JsonDocument.Parse("{\"hello\": \"world\"}").RootElement;
        var model = new AppJsonDataModel {
            BusinessId = DateTime.Now.ToUnixTime().ToString(),
            Name = "test",
            Value = val
        };
        await Target.SaveAsync(model);
        Assert.That(model.Id, Is.Not.Empty);
        var model2 = await Target.GetByIdAsync(long.Parse(model.Id));
        Assert.That(model2, Is.Not.Null);
        Console.WriteLine(val.ToJson());
        Console.WriteLine(model2.ToJson());
        await Target.DeleteAsync(long.Parse(model.Id));
    }

    [Test]
    public void _05_CanMapAppJsonData() {
        var entity = new AppJsonData {
            Id = 123456L,
            BusinessId = 123456L,
            Name = "test",
            Value = System.Text.Json.JsonDocument.Parse("{}").RootElement
        };
        var mapper = ServiceProvider.GetService<AutoMapper.IMapper>();
        var model = mapper.Map<AppJsonDataModel>(entity);
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Id, Is.EqualTo(entity.Id.ToString()));
        entity = mapper.Map<AppJsonData>(model);
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.Id, Is.EqualTo(long.Parse(model.Id)));
    }

}
