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
        /// <summary>数据源</summary>
        [Required(ErrorMessage = "数据源 必须填写！")]
        public StringIdNameEntity DataSource { get; set; }
        /// <summary>是否向数据源写入数据</summary>
        [Required(ErrorMessage = "是否向数据源写入数据 必须填写！")]
        public bool WriteData { get; set; }
        /// <summary>数据API调用的 XML + SQL 命令</summary>
        [Required(ErrorMessage = "数据API调用的 XML + SQL 命令 必须填写！")]
        public string Statement { get; set; }
        /// <summary>参数定义</summary>
        [Required(ErrorMessage = "参数定义 必须填写！")]
        public DataApiParameterModel[] Parameters { get; set; }
        /// <summary>API 输出列的源数据</summary>
        public DataServiceFieldModel[] Columns { get; set; }
        /// <summary>允许访问的角色</summary>
        public string[] Roles { get; set; }
        /// <summary>创建者</summary>
        public StringIdNameEntity Creator { get; set; }
        /// <summary>创建时间</summary>
        [Required(ErrorMessage = "创建时间 必须填写！")]
        public DateTime CreatedAt { get; set; }
        /// <summary>更新者</summary>
        public StringIdNameEntity Updater { get; set; }
        /// <summary>更新时间</summary>
        [Required(ErrorMessage = "更新时间 必须填写！")]
        public DateTime UpdatedAt { get; set; }

    }

    public class DataApiParameterModel {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public bool Required { get; set; }
    }

    /// <summary>数据API搜索参数</summary>
    public partial class DataApiSearchModel : PaginatedRequestModel { }

}
