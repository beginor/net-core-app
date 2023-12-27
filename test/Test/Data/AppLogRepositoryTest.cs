﻿using System.Threading.Tasks;
using NUnit.Framework;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test.Data;

/// <summary>应用程序日志仓储测试</summary>
[TestFixture]
public class AppLogRepositoryTest : BaseTest<IAppLogRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.That(Target, Is.Not.Null);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new AppLogSearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.That(result.Total, Is.GreaterThanOrEqualTo(0));
        Assert.That(result.Take, Is.GreaterThanOrEqualTo(result.Data.Count));
    }
}
