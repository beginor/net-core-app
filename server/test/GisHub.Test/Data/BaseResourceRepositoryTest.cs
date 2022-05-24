using System;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Test.Data;

/// <summary>数据资源的基类仓储测试</summary>
[TestFixture]
public class BaseResourceRepositoryTest : BaseTest<IBaseResourceRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new BaseResourceSearchModel {
            Skip = 0,
            Take = 100
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.GreaterOrEqual(result.Total, 0);
        Assert.GreaterOrEqual(result.Take, result.Data.Count);
    }
    
    [Test]
    public async Task _03_CanCountByCategoryAsync() {
        var model = new BaseResourceStatisticRequestModel();
        var result = await Target.CountByCategoryAsync(model);
        Assert.IsNotEmpty(result.Data);
        Console.WriteLine(result.ToJson());
        //
        model.Type = "data_api";
        result = await Target.CountByCategoryAsync(model);
        Assert.IsNotEmpty(result.Data);
        Console.WriteLine(result.ToJson());
    }
    
    [Test]
    public async Task _04_CanGetRolesByResourceId() {
        var resourceId = 1618394546378030029;
        var roles = await Target.GetRolesByResourceIdAsync(resourceId);
        Assert.IsNotEmpty(roles);
        Console.WriteLine(string.Join(',', roles));
    }

}
