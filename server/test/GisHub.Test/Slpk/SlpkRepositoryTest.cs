using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.NetCore;
using NUnit.Framework;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;
using Beginor.GisHub.Slpk.Data;
using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Test.Slpk;

/// <summary>slpk 航拍模型仓储测试</summary>
[TestFixture]
public class SlpkRepositoryTest : BaseTest<ISlpkRepository> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

    [Test]
    public async Task _02_CanDoSearchAsync() {
        var searchModel = new SlpkSearchModel {
            Skip = 0,
            Take = 10
        };
        var result = await Target.SearchAsync(searchModel);
        Assert.GreaterOrEqual(result.Total, 0);
        Assert.GreaterOrEqual(result.Take, result.Data.Count);
    }

}
