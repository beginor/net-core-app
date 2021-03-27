using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.TileMap.Models {

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
    }

    /// <summary>矢量切片包搜索参数</summary>
    public partial class VectorTileSearchModel : PaginatedRequestModel { }

}
