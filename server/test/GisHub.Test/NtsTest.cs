using System;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
using NetTopologySuite.Geometries;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.Geo;
using Beginor.GisHub.Geo.GeoJson;
using NetTopologySuite.IO;

namespace Beginor.GisHub.Test;

[TestFixture]
public class NtsTest {

    [Test]
    public void _01_CanSerializePoint() {
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web
        );
        options.Converters.Add(new GeoJsonGeometryConverter(new CoordinateConverter()));
        options.Converters.Add(new CoordinateConverter());
        var point2D = new Point(113.2, 23.40000444);
        var arr2D = point2D.Coordinate.ToArray();
        Assert.AreEqual(arr2D.Length, 2);
        Console.WriteLine(point2D.ToGeoJson().ToJson(options));
        var point3D = new Point(113.2, 23.4, 100);
        var arr3D = point3D.Coordinate.ToArray();
        Assert.AreEqual(arr3D.Length, 3);
        Console.WriteLine(point3D.ToGeoJson().ToJson(options));
        GeoJsonGeometry geom = new GeoJsonPoint {
            Coordinates = new[] { 113.2, 23.4 }
        };
        Console.WriteLine(JsonSerializer.Serialize( new GeoJsonFeature { Geometry = geom }, options));
    }

    [Test]
    public void _02_CanReadPolygon() {
        var wkt = File.ReadAllText("wkt/p2.wkt");
        Console.WriteLine(wkt);
        var reader = new WKTReader();
        var geom = reader.Read(wkt);
        var polygon = geom as Polygon;
        Assert.IsNotNull(polygon);
        Console.WriteLine(polygon.ToGeoJson().ToJson());
        Console.WriteLine(polygon.ToAgs().ToJson());
    }

    [Test]
    public void _03_CanReadPolygonWithHole() {
        var wkt = File.ReadAllText("wkt/p4.wkt");
        Console.WriteLine(wkt);
        var reader = new WKTReader();
        var geom = reader.Read(wkt);
        var polygon = geom as Polygon;
        Assert.IsNotNull(polygon);
        Console.WriteLine(polygon.ToGeoJson().ToJson());
        Console.WriteLine(polygon.ToAgs().ToJson());
    }

    [Test]
    public void _04_CanReadMultiPolygon() {
        var wkt = File.ReadAllText("wkt/p6.wkt");
        Console.WriteLine(wkt);
        var reader = new WKTReader();
        var geom = reader.Read(wkt);
        var polygon = geom as MultiPolygon;
        Assert.IsNotNull(polygon);
        Console.WriteLine(polygon.ToGeoJson().ToJson());
        Console.WriteLine(polygon.ToAgs().ToJson());
    }

    [Test, Ignore("Nts wkt does not support BOX")]
    public void _05_CanReadBBox() {
        var wkt = File.ReadAllText("wkt/bbox.wkt");
        Console.WriteLine(wkt);
        var reader = new WKTReader();
        var geom = reader.Read(wkt);
        Console.WriteLine(geom.GetType());
    }

    [Test, Ignore("Nts wkt does not support MULTISURFACE")]
    public void _06_CanReadMultiSurface() {
        var wkt = File.ReadAllText("wkt/multisurface.wkt");
        Console.WriteLine(wkt);
        var reader = new WKTReader();
        var geom = reader.Read(wkt);
        Console.WriteLine(geom.GetType());
    }

    [Test]
    public void _07_CanComputeExtent() {
        using var wkt = new StreamReader("wkt/gd_buffer.wkt");
        var reader = new WKTReader();
        var geom = reader.Read(wkt) as MultiPolygon;
        Assert.IsNotNull(geom);
        var envelop = geom.EnvelopeInternal;
        Assert.IsNotNull(envelop);
        var minX = envelop.MinX;
        var maxX = envelop.MaxX;
        var minY = envelop.MinY;
        var maxY = envelop.MaxY;
        var z = 19;
        var minCol = Gmap.Utils.MercatorTileUtil.Lng2TileX(minX, z);
        var minRow = Gmap.Utils.MercatorTileUtil.Lat2TileY(minY, z);
        var maxCol = Gmap.Utils.MercatorTileUtil.Lng2TileX(maxX, z);
        var maxRow = Gmap.Utils.MercatorTileUtil.Lat2TileY(maxY, z);
        Console.WriteLine($"z: {z}, minCol: {minCol}, minRow: {minRow}, maxCol: {maxCol}, maxRow: {maxRow}");
    }

}
