using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.TileMap.Models; 

/// <summary>矢量切片包模型</summary>
public partial class VectorTileModel : StringEntity {

    /// <summary>矢量切片包名称</summary>
    [Required(ErrorMessage = "矢量切片包名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>矢量切片包目录</summary>
    [Required(ErrorMessage = "矢量切片包目录 必须填写！")]
    public string Directory { get; set; }
    /// <summary>最小缩放级别</summary>
    public short? MinZoom { get; set; }
    /// <summary>最大缩放级别</summary>
    public short? MaxZoom { get; set; }
    /// <summary>默认样式</summary>
    public string DefaultStyle { get; set; }
    /// <summary>样式内容</summary>
    public string StyleContent { get; set; }
    /// <summary>最小纬度</summary>
    public double? MinLatitude { get; set; }
    /// <summary>最大纬度</summary>
    public double? MaxLatitude { get; set; }
    /// <summary>最小经度</summary>
    public double? MinLongitude { get; set; }
    /// <summary>最大经度</summary>
    public double? MaxLongitude { get; set; }
}

/// <summary>矢量切片包搜索参数</summary>
public partial class VectorTileSearchModel : PaginatedRequestModel {
    /// <summary>搜索关键字</summary>
    public string Keywords { get; set; }
}