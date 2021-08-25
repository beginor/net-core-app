using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models {

    /// <summary>用户访问凭证模型</summary>
    public partial class UserAccessTokenModel : StringEntity {

        /// <summary>用户id</summary>
        [Required(ErrorMessage = "用户id 必须填写！")]
        public string UserId { get; set; }
        /// <summary>凭证名称</summary>
        [Required(ErrorMessage = "凭证名称 必须填写！")]
        public string Name { get; set; }
        /// <summary>凭证值</summary>
        [Required(ErrorMessage = "凭证值 必须填写！")]
        public string Value { get; set; }
        /// <summary>凭证权限</summary>
        public string[] Privileges { get; set; }
        /// <summary>允许的 url 地址</summary>
        public string[] Urls { get; set; }
        /// <summary>更新时间</summary>
        [Required(ErrorMessage = "更新时间 必须填写！")]
        public DateTime UpdateTime { get; set; }

    }

    /// <summary>用户访问凭证搜索参数</summary>
    public partial class UserAccessTokenSearchModel : PaginatedRequestModel { }

}