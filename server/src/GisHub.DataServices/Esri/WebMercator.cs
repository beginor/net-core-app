

using System;

namespace Beginor.GisHub.DataServices.Esri {

    public static class WebMercator {

        private const double RadiansPrDegrees = 0.017453292519943295;

        private const double DegreesPrRadians = 57.295779513082323;

        private const double Wgs84EarthRadius = 6378137.0;

        /// <summary>
        /// Helper method for quickly projecting coordinates from
        /// geographic WGS84(Wkid=4326) coordinates to web mercator(Wkid=102100).
        /// </summary>
        public static AgsGeometry FromWgs84(AgsGeometry geometry) {
            if (geometry.SpatialReference.Wkid != 4326) {
                throw new InvalidOperationException("WkId != 4326");
            }
            return FromGeographic(geometry);
        }
        /// <summary>
        /// Helper method for quickly projecting coordinates from
        /// geographic CGC2000(Wkid=4490) coordinates to web mercator(Wkid=102100).
        /// </summary>
        public static AgsGeometry FromCgc2000(
            AgsGeometry geometry
        ) {
            if (geometry.SpatialReference.Wkid != 4490) {
                throw new InvalidOperationException("WkId != 4490");
            }
            return FromGeographic(geometry);
        }

        private static AgsGeometry FromGeographic(
            AgsGeometry geometry
        ) {
            AgsGeometry result = null;
            if (geometry is AgsPoint p) {
                var xy = GeographicToWebMercator(new[] {p.X, p.Y});
                result = new AgsPoint {
                    X = xy[0],
                    Y = xy[1]
                };
            }
            else if (geometry is AgsMultiPoint mp) {
                result = new AgsMultiPoint {
                    Points = GeographicToWebMercator(mp.Points)
                };
            }
            else if (geometry is AgsPolyline pl) {
                result = new AgsPolyline {
                    Paths = GeographicToWebMercator(pl.Paths)
                };
            }
            else if (geometry is AgsPolygon pg) {
                result = new AgsPolygon {
                    Rings = GeographicToWebMercator(pg.Rings)
                };
            }
            else if (geometry is AgsExtent e) {
                var xys = new[] {
                    new[] {e.Xmin, e.Ymin}, new[] {e.Xmax, e.Ymax}
                };
                var txys = GeographicToWebMercator(xys);
                result = new AgsExtent {
                    Xmin = txys[0][0],
                    Ymin = txys[0][1],
                    Xmax = txys[1][0],
                    Ymax = txys[1][1]
                };
            }
            if (result != null) {
                result.SpatialReference = new AgsSpatialReference {
                    Wkid = 102100,
                    LatestWkid = 3857
                };
            }
            return result;
        }

        /// Helper method for quickly unprojecting coordinates from
        /// webmercator (WKID=102100) to geographic WGS84 coordinates (WKID=4326).
        public static AgsGeometry ToWgs84(AgsGeometry geometry) {
            if (geometry.SpatialReference?.Wkid != 102100 && geometry.SpatialReference?.Wkid != 3857) {
                throw new InvalidOperationException("WkId != 102100 or 3857");
            }
            return ToGeographic(geometry, new AgsSpatialReference { Wkid = 4326 });
        }

        /// Helper method for quickly unprojecting coordinates from
        /// webmercator (WKID=102100) to geographic CGC2000 coordinates (WKID=4490).
        public static AgsGeometry ToCgc2000(AgsGeometry geometry) {
            if (geometry.SpatialReference?.Wkid != 102100 && geometry.SpatialReference?.Wkid != 3857) {
                throw new InvalidOperationException("WkId != 102100 or 3857");
            }
            return ToGeographic(geometry, new AgsSpatialReference { Wkid = 4490 });
        }

        private static AgsGeometry ToGeographic(
            AgsGeometry geometry,
            AgsSpatialReference spatialReference
        ) {
            AgsGeometry result = null;
            if (geometry is AgsPoint point) {
                var p = point;
                var xy = WebMercatorToGeographic(new[] {p.X, p.Y});
                result = new AgsPoint {
                    X = xy[0],
                    Y = xy[1]
                };
            }
            else if (geometry is AgsMultiPoint mp) {
                result = new AgsMultiPoint {
                    Points = WebMercatorToGeographic(mp.Points)
                };
            }
            else if (geometry is AgsPolyline pl) {
                result = new AgsPolyline {
                    Paths = WebMercatorToGeographic(pl.Paths)
                };
            }
            else if (geometry is AgsPolygon pg) {
                result = new AgsPolygon {
                    Rings = WebMercatorToGeographic(pg.Rings)
                };
            }
            else if (geometry is AgsExtent e) {
                var xys = new[] {
                    new[] {e.Xmin, e.Ymin},
                    new[] {e.Xmax, e.Ymax}
                };
                var txys = WebMercatorToGeographic(xys);
                result = new AgsExtent {
                    Xmin = txys[0][0],
                    Ymin = txys[0][1],
                    Xmax = txys[1][0],
                    Ymax = txys[1][1]
                };
            }
            if (result != null) {
                result.SpatialReference = spatialReference;
            }
            return result;
        }

        private static double[] GeographicToWebMercator(double[] point) {
            if (point == null) {
                return null;
            }
            double y;
            if (point[1] < -90.0) {
                y = double.NegativeInfinity;
            }
            else if (point[1] > 90.0) {
                y = double.PositiveInfinity;
            }
            else {
                var num = point[1] * RadiansPrDegrees;
                y = 3189068.5 * Math.Log((1.0 + Math.Sin(num)) / (1.0 - Math.Sin(num)));
            }
            var x = Wgs84EarthRadius * (point[0] * RadiansPrDegrees);
            return new[] {x, y};
        }

        private static double[][] GeographicToWebMercator(double[][] points) {
            var result = new double[points.Length][];
            for (var i = 0; i < points.Length; i++) {
                result[i] = GeographicToWebMercator(points[i]);
            }
            return result;
        }

        private static double[][][] GeographicToWebMercator(
            double[][][] paths
        ) {
            var result = new double[paths.Length][][];
            for (var i = 0; i < paths.Length; i++) {
                result[i] = WebMercatorToGeographic(paths[i]);
            }
            return result;
        }

        private static double[] WebMercatorToGeographic(double[] point) {
            if (point == null) {
                return null;
            }
            var x = point[0];
            var y = point[1];
            var num = x / Wgs84EarthRadius;
            var num2 = num * DegreesPrRadians;
            var num3 = Math.Floor((num2 + 180.0) / 360.0);
            var num4 = num2 - num3 * 360.0;
            var num5 = 1.5707963267948966 - 2.0 * Math.Atan(Math.Exp(-1.0 * y / Wgs84EarthRadius));
            num5 *= DegreesPrRadians;
            return new[] {num4 + num3 * 360.0, num5};
        }

        private static double[][] WebMercatorToGeographic(double[][] points) {
            var result = new double[points.Length][];
            for (var i = 0; i < points.Length; i++) {
                result[i] = WebMercatorToGeographic(points[i]);
            }
            return result;
        }

        private static double[][][] WebMercatorToGeographic(
            double[][][] paths
        ) {
            var result = new double[paths.Length][][];
            for (var i = 0; i < paths.Length; i++) {
                result[i] = WebMercatorToGeographic(paths[i]);
            }
            return result;
        }

    }

}
