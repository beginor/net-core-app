using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.GeoJson {

    public static class NtsExtensions {

        public static GeoJsonGeometry ToGeoJson(
            this Geometry geom
        ) {
            switch (geom.GeometryType) {
                case Geometry.TypeNamePoint:
                    var p = (Point) geom;
                    return p.ToGeoJson();
                case Geometry.TypeNameMultiPoint:
                    var mp = (MultiPoint) geom;
                    return mp.ToGeoJson();
                case Geometry.TypeNameLineString:
                    var ls = (LineString) geom;
                    return ls.ToGeoJson();
                case Geometry.TypeNameMultiLineString:
                    var mls = (MultiLineString) geom;
                    return mls.ToGeoJson();
                case Geometry.TypeNamePolygon:
                    var polygon = (Polygon) geom;
                    return polygon.ToGeoJson();
                case Geometry.TypeNameMultiPolygon:
                    var mpl = (MultiPolygon) geom;
                    return mpl.ToGeoJson();
                case Geometry.TypeNameLinearRing:
                    var ring = (LinearRing) geom;
                    return ring.ToGeoJson();
                case Geometry.TypeNameGeometryCollection:
                    var collection = (GeometryCollection) geom;
                    return collection.ToGeoJson();
                default:
                    throw new NotSupportedException(
                        $"Not supported {geom.GeometryType} !"
                    );
            }
        }

        public static GeoJsonGeometryCollection ToGeoJson(
            this GeometryCollection collection
        ) {
            var list = new List<GeoJsonGeometry>();
            var count = collection.NumGeometries;
            for (var i = 0; i < count; i++) {
                list.Add(collection.GetGeometryN(i).ToGeoJson());
            }
            return new GeoJsonGeometryCollection { Geometries = list.ToArray() };
        }

        public static GeoJsonPolygon ToGeoJson(
            this Polygon polygon
        ) {
            var list = new List<double[][]> {
                polygon.ExteriorRing.ToGeoJson().Coordinates
            };
            list.AddRange(
                polygon.InteriorRings.Select(r => r.ToGeoJson().Coordinates)
            );
            return new GeoJsonPolygon { Coordinates = list.ToArray() };
        }

        public static GeoJsonPolygon ToGeoJson(
            this LinearRing ring
        ) {
            var list = new List<double[]>(ring.Coordinates.Length);
            list.AddRange(ring.Coordinates.Select(c => c.ToArray()));
            return new GeoJsonPolygon { Coordinates = new [] { list.ToArray() } };
        }

        public static GeoJsonMultiPolygon ToGeoJson(
            this MultiPolygon multiPolygon
        ) {
            var count = multiPolygon.NumGeometries;
            var list = new List<double[][][]>(count);
            for (var i = 0; i < count; i++) {
                var polygon = (Polygon) multiPolygon.GetGeometryN(i);
                list.Add(polygon.ToGeoJson().Coordinates);
            }
            return new GeoJsonMultiPolygon { Coordinates = list.ToArray() };
        }

        public static GeoJsonLineString ToGeoJson(
            this LineString lineString
        ) {
            var list = new List<double[]>(lineString.Coordinates.Length);
            list.AddRange(lineString.Coordinates.Select(c => c.ToArray()));
            return new GeoJsonLineString { Coordinates = list.ToArray() };
        }

        public static GeoJsonMultiLineString ToGeoJson(
            this MultiLineString multiLine
        ) {
            var lineCount = multiLine.NumGeometries;
            var coords = new List<double[][]>(lineCount);
            for (var i = 0; i < lineCount; i++) {
                var line = (LineString)multiLine.GetGeometryN(i);
                var jsonLine = line.ToGeoJson();
                coords.Add(jsonLine.Coordinates);
            }
            return new GeoJsonMultiLineString { Coordinates = coords.ToArray() };
        }

        public static GeoJsonPoint ToGeoJson(
            this Point point
        ) {
            return new GeoJsonPoint { Coordinates = point.Coordinate.ToArray() };
        }

        public static GeoJsonMultiPoint ToGeoJson(
            this MultiPoint multiPoint
        ) {
            var pointCount = multiPoint.NumGeometries;
            var list = new List<double[]>(pointCount);
            for (var i = 0; i < pointCount; i++) {
                var point = (Point) multiPoint.GetGeometryN(i);
                list.Add(point.Coordinate.ToArray());
            }
            return new GeoJsonMultiPoint { Coordinates = list.ToArray() };
        }

        public static double[] ToArray(
            this Coordinate coordinate
        ) {
            var list = new List<double> {
                coordinate.X,
                coordinate.Y
            };
            if (!double.IsNaN(coordinate.Z)) {
                list.Add(coordinate.Z);
            }
            return list.ToArray();
        }
    }

}
