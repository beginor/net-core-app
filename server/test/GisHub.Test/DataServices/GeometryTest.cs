using System;
using System.Text.Json;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.Test.DataServices;

[TestFixture]
public class GeometryTest : BaseTest<CommonOption> {

    [Test]
    public void _01_CanResolveTarget() {
        IsNotNull(Target);
    }

    [Test]
    public void _02_CanDeserializeGeometry() {
        var factory = new JsonSerializerOptionsFactory(Target);
        var json = "{\"spatialReference\":{\"latestWkid\":3857,\"wkid\":102100},\"xmin\":11271098.442813028,\"ymin\":2504688.542850986,\"xmax\":11897270.578525025,\"ymax\":3130860.6785629876}";
        var extent = JsonSerializer.Deserialize<AgsExtent>(json, factory.AgsJsonSerializerOptions);
        IsNotNull(extent);
        var wkt = extent.ToNtsGeometry().AsText();
        IsNotNull(wkt);
        Console.WriteLine(wkt);
    }
}
