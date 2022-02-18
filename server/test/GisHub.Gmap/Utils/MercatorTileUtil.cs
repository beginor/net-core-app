using System;

namespace Beginor.GisHub.Gmap.Utils; 

/// <summary>墨卡托切片工具类</summary>
public static class MercatorTileUtil {

    public static int Lng2TileX(double lng, int z) {
        return (int)(Math.Floor((lng + 180.0) / 360.0 * (1 << z)));
    }

    public static int Lat2TileY(double lat, int z) {
        return (int)Math.Floor((1 - Math.Log(Math.Tan(ToRadians(lat)) + 1 / Math.Cos(ToRadians(lat))) / Math.PI) / 2 * (1 << z));
    }

    public static double TileX2Lng(int x, int z) {
        return x / (double)(1 << z) * 360.0 - 180;
    }

    public static double TileY2Lat(int y, int z) {
        double n = Math.PI - 2.0 * Math.PI * y / (double)(1 << z);
        return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
    }

    private static double ToRadians(double deg) {
        return deg * Math.PI / 180.0;
    }

}