using System;
using System.Text.Json;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Esri;
using NUnit.Framework;

namespace Beginor.GisHub.Test.DataServices {

    [TestFixture]
    public class GeometryTest {

        [Test]
        public void CanDeserializeGeometry() {
            var json = "{\"spatialReference\":{\"latestWkid\":3857,\"wkid\":102100},\"xmin\":11271098.442813028,\"ymin\":2504688.542850986,\"xmax\":11897270.578525025,\"ymax\":3130860.6785629876}";
            var extent = JsonSerializer.Deserialize<AgsExtent>(json, JsonFactory.CreateAgsJsonSerializerOptions());
            Assert.IsNotNull(extent);
            var wkt = extent.ToNtsGeometry().AsText();
            Assert.IsNotNull(wkt);
            Console.WriteLine(wkt);
        }
    }
}
