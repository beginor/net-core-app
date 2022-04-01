using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.WebEncoders.Testing;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Test.Data;

/// <summary>数据类别仓储测试</summary>
[TestFixture]
public class CategoryRepositoryTest : BaseTest<ICategoryRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new CategorySearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.GreaterOrEqual(result.Total, 0);
        Assert.GreaterOrEqual(result.Take, result.Data.Count);
    }

    [Test]
    public async Task _03_CanGetAll() {
        var result = await Target.GetAllAsync();
        Assert.GreaterOrEqual(result.Count, 0);
        Console.WriteLine(result.ToJson(new JsonSerializerOptions { WriteIndented = true, Encoder = new JavaScriptTestEncoder() }));
    }

}
