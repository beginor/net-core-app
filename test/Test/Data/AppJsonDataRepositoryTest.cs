using System;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.AppFx.Core;
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
        var id = DateTime.Now.ToUnixTime();
        var val = System.Text.Json.JsonDocument.Parse("{\"hello\": \"world\"}").RootElement;
        await Target.SaveValueAsync(id, val);
        var val2 = await Target.GetValueByIdAsync(id);
        Assert.That(val2, Is.Not.Null);
        Console.WriteLine(val.ToJson());
        Console.WriteLine(val2.ToJson());
        await Target.DeleteAsync(id);
    }

    [Test]
    public async Task _04_CanQueryEmptyAsync() {
        var id = DateTime.Now.ToUnixTime();
        var val = await Target.GetValueByIdAsync(id);
        Assert.That(val, Is.Not.Null);
        Console.WriteLine(val);
        Assert.That(val.ValueKind, Is.EqualTo(JsonValueKind.Undefined));
    }

}
