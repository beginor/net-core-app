using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models; 

#nullable disable

/// <summary>用户凭证模型</summary>
public partial class AppUserTokenModel : StringEntity {

    /// <summary>凭证名称</summary>
    [Required(ErrorMessage = "凭证名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>凭证值</summary>
    [Required(ErrorMessage = "凭证值 必须填写！")]
    public string Value { get; set; }
    /// <summary>凭证角色</summary>
    public string[] Roles { get; set; }
    /// <summary>凭证权限</summary>
    public string[] Privileges { get; set; }
    /// <summary>允许的 url 地址</summary>
    public string[] Urls { get; set; }
    /// <summary>过期时间</summary>
    public DateTime? ExpiresAt { get; set; }
    /// <summary>更新时间</summary>
    [Required(ErrorMessage = "更新时间 必须填写！")]
    public DateTime UpdateTime { get; set; }

}

/// <summary>用户凭证搜索参数</summary>
public partial class AppUserTokenSearchModel : PaginatedRequestModel {
    /// <summary>查询关键字</summary>
    public string? Keywords { get; set; }
}
