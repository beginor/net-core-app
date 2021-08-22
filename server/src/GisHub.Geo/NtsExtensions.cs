using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using Beginor.GisHub.Geo.GeoJson;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.Geo {

    public static class NtsExtensions {

        #region "Convert NtsGeometry to Ags Geometry"
        public static AgsGeometry ToAgs(
            this Geometry geom
        ) {
            switch (geom.GeometryType) {
                case Geometry.TypeNamePoint:
                    var p = (Point) geom;
                    return p.ToAgs();
                case Geometry.TypeNameMultiPoint:
                    var mp = (MultiPoint) geom;
                    return mp.ToAgs();
                case Geometry.TypeNameLineString:
                    var ls = (LineString) geom;
                    return ls.ToAgs();
                case Geometry.TypeNameMultiLineString:
                    var mls = (MultiLineString) geom;
                    return mls.ToAgs();
                case Geometry.TypeNamePolygon:
                    var polygon = (Polygon) geom;
                    return polygon.ToAgs();
                case Geometry.TypeNameMultiPolygon:
                    var mpl = (MultiPolygon) geom;
                    return mpl.ToAgs();
                case Geometry.TypeNameLinearRing:
                    var ring = (LinearRing) geom;
                    return ring.ToAgs();
                default:
                    throw new NotSupportedException(
                        $"Not supported {geom.GeometryType} !"
                    );
            }
        }

        public static AgsPoint ToAgs(
            this Point point
        ) {
            return new AgsPoint {
                X = point.X,
                Y = point.Y,
                Z = double.IsNaN(point.Z) ? null : point.Z,
                M = double.IsNaN(point.M) ? null : point.M,
                HasZ = double.IsNaN(point.Z) ? null : true,
                HasM = double.IsNaN(point.M) ? null : true
            };
        }

        public static AgsMultiPoint ToAgs(
            this MultiPoint multiPoint
        ) {
            var pointCount = multiPoint.NumGeometries;
            var list = new List<double[]>(pointCount);
            for (var i = 0; i < pointCount; i++) {
                var point = (Point) multiPoint.GetGeometryN(i);
                list.Add(point.Coordinate.ToArray());
            }
            return new AgsMultiPoint {
                Points = list.ToArray(),
                HasZ = double.IsNaN(multiPoint.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(multiPoint.Coordinate.M) ? null : true
            };
        }

        public static AgsPolyline ToAgs(
            this LineString lineString
        ) {
            var list = new List<double[]>(lineString.Coordinates.Length);
            list.AddRange(lineString.Coordinates.Select(c => c.ToArray()));
            return new AgsPolyline {
                Paths = new[] { list.ToArray() },
                HasZ = double.IsNaN(lineString.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(lineString.Coordinate.M) ? null : true
            };
        }

        public static AgsPolyline ToAgs(
            this MultiLineString multiLine
        ) {
            var lineCount = multiLine.NumGeometries;
            var coords = new List<double[][]>(lineCount);
            for (var i = 0; i < lineCount; i++) {
                var line = (LineString)multiLine.GetGeometryN(i);
                var jsonLine = line.ToGeoJson();
                coords.Add(jsonLine.Coordinates);
            }
            return new AgsPolyline {
                Paths = coords.ToArray(),
                HasZ = double.IsNaN(multiLine.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(multiLine.Coordinate.M) ? null : true
            };
        }

        public static AgsPolygon ToAgs(
            this Polygon polygon
        ) {
            var list = new List<double[][]> {
                polygon.ExteriorRing.ToGeoJson().Coordinates
            };
            list.AddRange(
                polygon.InteriorRings.Select(r => r.ToGeoJson().Coordinates)
            );
            return new AgsPolygon {
                Rings = list.ToArray(),
                HasZ = double.IsNaN(polygon.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(polygon.Coordinate.M) ? null : true
            };
        }

        public static AgsPolygon ToAgs(
            this LinearRing ring
        ) {
            var list = new List<double[]>(ring.Coordinates.Length);
            list.AddRange(ring.Coordinates.Select(c => c.ToArray()));
            return new AgsPolygon {
                Rings = new[] { list.ToArray() },
                HasZ = double.IsNaN(ring.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(ring.Coordinate.M) ? null : true
            };
        }

        public static AgsPolygon ToAgs(
            this MultiPolygon multiPolygon
        ) {
            var list = new List<double[][]>();
            var count = multiPolygon.NumGeometries;
            for (var i = 0; i < count; i++) {
                var polygon = (Polygon) multiPolygon.GetGeometryN(i);
                list.Add(polygon.ExteriorRing.ToGeoJson().Coordinates);
                list.AddRange(
                    polygon.InteriorRings.Select(r => r.ToGeoJson().Coordinates)
                );
            }
            return new AgsPolygon {
                Rings = list.ToArray(),
                HasZ = double.IsNaN(multiPolygon.Coordinate.Z) ? null : true,
                HasM = double.IsNaN(multiPolygon.Coordinate.M) ? null : true
            };
        }

        #endregion

        #region "Convert NtsGeometry to GeoJson Geometry"
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
        #endregion

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
            if (!double.IsNaN(coordinate.M)) {
                list.Add(coordinate.M);
            }
            return list.ToArray();
        }
    }

}
