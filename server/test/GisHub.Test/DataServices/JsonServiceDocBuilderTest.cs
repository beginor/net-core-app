using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.Test.DataServices;

[TestFixture]
public class JsonServiceDocBuilderTest : BaseTest<JsonServiceDocBuilder> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }
    
    [Test]
    public async Task _02_CanBuildDoc() {
        var repo = ServiceProvider.GetService<IDataServiceRepository>();
        var services = new List<DataServiceCacheItem> {
            await repo.GetCacheItemByIdAsync(1632716195306030468),
            await repo.GetCacheItemByIdAsync(1625465201773030068)
        };

        var docModel = new DocModel<DataServiceCacheItem> {
            Title = "数据服务文档测试",
            Description = "测试导出数据服务文档测试",
            BaseUrl = "http://localhost:5000/gishub/api",
            Models = services,
            Token = "12345678",
            Referer = "http://localhost:3000"
        };
        
        var result = Target.BuildApiDoc(docModel);
        Console.WriteLine(result);
        Assert.IsNotEmpty(result);
    }

}
