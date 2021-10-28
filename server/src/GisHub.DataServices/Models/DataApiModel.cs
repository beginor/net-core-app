using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Models {

    /// <summary>数据API模型</summary>
    public partial class DataApiModel : StringEntity {

        /// <summary>数据API名称</summary>
        [Required(ErrorMessage = "数据API名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>数据API描述</summary>
        public string Description { get; set; }
        /// <summary>数据源ID</summary>
        [Required(ErrorMessage = "数据源ID 必须填写！")]
        public string DataSourceId { get; set; }
        /// <summary>是否向数据源写入数据</summary>
        [Required(ErrorMessage = "是否向数据源写入数据 必须填写！")]
        public bool WriteData { get; set; }
        /// <summary>数据API调用的 XML + SQL 命令</summary>
        [Required(ErrorMessage = "数据API调用的 XML + SQL 命令 必须填写！")]
        public string Statement { get; set; }
        /// <summary>参数定义</summary>
        [Required(ErrorMessage = "参数定义 必须填写！")]
        public string Parameters { get; set; }
        /// <summary>API 输出列的源数据</summary>
        public string Columns { get; set; }
        /// <summary>允许访问的角色</summary>
        public string[] Roles { get; set; }
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

    /// <summary>数据API搜索参数</summary>
    public partial class DataApiSearchModel : PaginatedRequestModel { }

}