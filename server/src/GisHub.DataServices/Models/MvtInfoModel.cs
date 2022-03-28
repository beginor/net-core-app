namespace Beginor.GisHub.DataServices.Models;

/// <summary>矢量切片信息</summary>
public class MvtInfoModel {
    /// <summary>图层名称</summary>
    public string LayerName { get; set; }
    /// <summary>图层说明</summary>
    public string Description { get; set; }
    /// <summary>空间数据类型</summary>
    public string GeometryType { get; set; }
    /// <summary>最小缩放级别</summary>
    public int Minzoom { get; set; }
    /// <summary>最大缩放级别</summary>
    public int Maxzoom { get; set; }
    /// <summary>数据范围</summary>
    public double[] Bounds { get; set; }
    /// <summary>地址</summary>
    public string Url { get; set; }
}
