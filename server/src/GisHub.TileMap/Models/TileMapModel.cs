using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.TileMap.Models {

    /// <summary>切片地图模型</summary>
    public partial class TileMapModel : StringEntity {

        /// <summary>切片地图名称</summary>
        [Required(ErrorMessage = "切片地图名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>缓存目录</summary>
        [Required(ErrorMessage = "缓存目录 必须填写！")]
        public string CacheDirectory { get; set; }
        /// <summary>切片信息路径</summary>
        [Required(ErrorMessage = "切片信息路径 必须填写！")]
        public string MapTileInfoPath { get; set; }
        /// <summary>内容类型</summary>
        [Required(ErrorMessage = "内容类型 必须填写！")]
        public string ContentType { get; set; }
        /// <summary>是否为紧凑格式</summary>
        [Required(ErrorMessage = "是否为紧凑格式 必须填写！")]
        public bool IsBundled { get; set; }
        /// <summary>最小缩放级别</summary>
        public short MinLevel { get; set; }
        /// <summary>最大缩放级别</summary>
        public short MaxLevel { get; set; }
        /// <summary>创建时间</summary>
        public virtual DateTime CreatedAt { get; set; }
        /// <summary>更新时间</summary>
        public virtual DateTime UpdatedAt { get; set; }

    }

    /// <summary>切片地图搜索参数</summary>
    public partial class TileMapSearchModel : PaginatedRequestModel {
        /// <summary>关键字，搜索图层名称或切片路径</summary>
        public string Keywords { get; set; }
    }

}
