using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Models {

    /// <summary>数据库连接串模型</summary>
    public partial class ConnectionStringModel : StringEntity {

        /// <summary>连接串名称</summary>
        [Required(ErrorMessage = "连接串名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>连接串值</summary>
        [Required(ErrorMessage = "连接串值 必须填写！")]
        public string Value { get; set; }
        /// <summary>数据库类型（postgres、mssql、mysql、oracle、sqlite等）</summary>
        [Required(ErrorMessage = "数据库类型（postgres、mssql、mysql、oracle、sqlite等） 必须填写！")]
        public string DatabaseType { get; set; }
        /// <summary>是否已删除（软删除）</summary>
        [Required(ErrorMessage = "是否已删除（软删除） 必须填写！")]
        public bool IsDeleted { get; set; }

    }

    /// <summary>数据库连接串搜索参数</summary>
    public partial class ConnectionStringSearchModel : PaginatedRequestModel { }

}
