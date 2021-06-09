using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Models {

    /// <summary>数据服务模型</summary>
    public partial class DataServiceModel : StringEntity {

        /// <summary>数据服务名称</summary>
        [Required(ErrorMessage = "数据服务名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>数据服务描述</summary>
        public string Description { get; set; }
        /// <summary>数据库连接</summary>
        [Required(ErrorMessage = "数据库连接 必须填写！")]
        public StringIdNameEntity DataSource { get; set; }
        /// <summary>数据表/视图架构</summary>
        public string Schema { get; set; }
        /// <summary>数据表/视图名称</summary>
        [Required(ErrorMessage = "数据表/视图名称 必须填写！")]
        public string TableName { get; set; }
        /// <summary>数据服务公开的列</summary>
        public DataServiceFieldModel[] Fields { get; set; }
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
        /// <summary>允许的角色</summary>
        public string[] Roles { get; set; }

    }

    /// <summary> 数据服务字段模型 </summary>
    public class DataServiceFieldModel {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public bool Nullable { get; set; }
        public bool Editable { get; set; }
    }

    /// <summary>数据服务搜索参数</summary>
    public partial class DataServiceSearchModel : PaginatedRequestModel {
        /// <summary>搜索数据服务的关键字</summary>
        public string Keywords { get; set; }
    }

}
