using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.VectorTile.Models {

    /// <summary>矢量切片包模型</summary>
    public partial class VectortileModel : StringEntity {

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
        /// <summary>创建者id</summary>
        [Required(ErrorMessage = "创建者id 必须填写！")]
        public string CreatorId { get; set; }
        /// <summary>创建时间</summary>
        [Required(ErrorMessage = "创建时间 必须填写！")]
        public DateTime CreatedAt { get; set; }
        /// <summary>更新者id</summary>
        [Required(ErrorMessage = "更新者id 必须填写！")]
        public string UpdaterId { get; set; }
        /// <summary>更新时间</summary>
        [Required(ErrorMessage = "更新时间 必须填写！")]
        public DateTime UpdatedAt { get; set; }
        /// <summary>是否删除</summary>
        [Required(ErrorMessage = "是否删除 必须填写！")]
        public bool IsDeleted { get; set; }

    }

    /// <summary>矢量切片包搜索参数</summary>
    public partial class VectortileSearchModel : PaginatedRequestModel { }

}
