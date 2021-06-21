using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>服务器目录模型</summary>
    public partial class ServerFolderModel : StringEntity {

        /// <summary>目录别名</summary>
        [Required(ErrorMessage = "目录别名 必须填写！")]
        public string AliasName { get; set; }
        /// <summary>根路径</summary>
        [Required(ErrorMessage = "根路径 必须填写！")]
        public string RootFolder { get; set; }
        /// <summary>是否只读</summary>
        [Required(ErrorMessage = "是否只读 必须填写！")]
        public bool Readonly { get; set; }
        /// <summary>roles, _varchar</summary>
        public string[] Roles { get; set; }

    }

    /// <summary>服务器目录搜索参数</summary>
    public partial class ServerFolderSearchModel : PaginatedRequestModel { }

}