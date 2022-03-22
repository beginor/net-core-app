using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.Test.DataServices; 

[TestFixture]
public class FeatureProviderTest : BaseTest {

    private IDataServiceFactory Factory => ServiceProvider.GetService<IDataServiceFactory>();

    [Test]
    public void _00_CanResolveTarget() {
        Assert.IsNotNull(Factory);
    }

    [Test]
    public async Task _01_CanQueryFeatures() {
        var repo = ServiceProvider.GetService<IDataServiceRepository>();
        Assert.IsNotNull(repo);
        var dataSource = await repo.GetCacheItemByIdAsync(1609887224871030614);
        Assert.IsNotNull(dataSource);
        var featureProvider = Factory.CreateFeatureProvider(dataSource.DatabaseType);
        Assert.IsNotNull(featureProvider);
        var param = new AgsQueryParam {
            Format = "json",
            Geometry = "{\"spatialReference\":{\"latestWkid\":3857,\"wkid\":102100},\"xmin\":11897270.578525025,\"ymin\":1878516.4071389847,\"xmax\":12523442.714237027,\"ymax\":2504688.542850986}",
            GeometryType = "esriGeometryEnvelope",
            MaxAllowableOffset = 1222.992452562501f,
            OrderByFields = "objectid ASC",
            OutFields = "objectid",
            OutSR = 102100,
            // ResultType = "tile",
            ReturnExceededLimitFeatures = false,
            SpatialRel = "esriSpatialRelIntersects",
            Where = "1=1",
            InSR = 102100
        };
        var featureset = await featureProvider.QueryAsync(dataSource, param);
        Assert.IsNotNull(featureset);
        Assert.IsNotEmpty(featureset.Features);
        Console.WriteLine(featureset.Features.Count);
        Assert.IsNotEmpty(featureset.Fields);
    }
}