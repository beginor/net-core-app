using System;
using System.Text.Json;
using NUnit.Framework;
using NetTopologySuite.Geometries;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.GeoJson;

namespace Beginor.GisHub.Test {

    [TestFixture]
    public class NtsTest {

        [Test]
        public void _01_CanSerializePoint() {
            var options = new System.Text.Json.JsonSerializerOptions(
                JsonSerializerDefaults.Web
            );
            var point2D = new Point(113.2, 23.4);
            var arr2D = point2D.Coordinate.ToArray();
            Assert.AreEqual(arr2D.Length, 2);
            Console.WriteLine(point2D.ToGeoJson().ToJson(options));
            var point3D = new Point(113.2, 23.4, 100);
            var arr3D = point3D.Coordinate.ToArray();
            Assert.AreEqual(arr3D.Length, 3);
            Console.WriteLine(point3D.ToGeoJson().ToJson(options));
        }

    }

}
