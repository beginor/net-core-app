using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Models {

    /// <summary>数据源（数据表或视图）模型</summary>
    public partial class DataSourceModel : StringEntity {

        /// <summary>数据源名称</summary>
        [Required(ErrorMessage = "数据源名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>数据库连接串id</summary>
        [Required(ErrorMessage = "数据库连接串id 必须填写！")]
        public string ConnectionStringId { get; set; }
        /// <summary>数据表/视图架构</summary>
        public string Schema { get; set; }
        /// <summary>数据表/视图名称</summary>
        [Required(ErrorMessage = "数据表/视图名称 必须填写！")]
        public string TableName { get; set; }
        /// <summary>主键列名称</summary>
        [Required(ErrorMessage = "主键列名称 必须填写！")]
        public string PrimaryKeyColumn { get; set; }
        /// <summary>显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。</summary>
        [Required(ErrorMessage = "显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。 必须填写！")]
        public string DisplayColumn { get; set; }
        /// <summary>空间列</summary>
        public string GeometryColumn { get; set; }
        /// <summary>预置过滤条件</summary>
        public string PresetCriteria { get; set; }
        /// <summary>默认排序</summary>
        public string DefaultOrder { get; set; }
        /// <summary>标签</summary>
        public string[] Tags { get; set; }
        /// <summary>是否删除</summary>
        [Required(ErrorMessage = "是否删除 必须填写！")]
        public bool IsDeleted { get; set; }

    }

    /// <summary>数据源（数据表或视图）搜索参数</summary>
    public partial class DataSourceSearchModel : PaginatedRequestModel { }

}
