using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

#nullable disable

namespace Beginor.NetCoreApp.Models;

/// <summary>应用程序用户模型</summary>
public class AppUserModel : StringEntity {
    /// <summary>用户名</summary>
    [Required(ErrorMessage = "用户名必须填写！")]
    public string UserName { get; set; }
    /// <summary>电子邮箱地址</summary>
    [Required(ErrorMessage = "电子邮箱地址必须填写！")]
    public string Email { get; set; }
    /// <summary>电子邮箱地址是否已确认</summary>
    public bool EmailConfirmed { get; set; }
    /// <summary>电话号码</summary>
    [Required(ErrorMessage = "电话号码须填写！")]
    public string PhoneNumber { get; set; }
    /// <summary>电话号码是否已经确认</summary>
    public bool PhoneNumberConfirmed { get; set; }
    /// <summary>是否允许（自动）锁定</summary>
    public bool LockoutEnabled { get; set; }
    /// <summary>锁定结束时间</summary>
    public DateTimeOffset? LockoutEnd { get; set; }
    /// <summary>登录失败次数</summary>
    public int AccessFailedCount { get; set; }
    /// <summary>是否启用两部认证</summary>
    public bool TwoFactorEnabled { get; }
    /// <summary>创建时间</summary>
    public DateTime CreateTime { get; set; }
    /// <summary>最近登录时间</summary>
    public DateTime? LastLogin { get; set; }
    /// <summary>登录次数</summary>
    public int LoginCount { get; set; }
    /// <summary>姓氏</summary>
    public string Surname { get; set; }
    /// <summary>名称</summary>
    public string GivenName { get; set; }
    /// <summary>出生日期</summary>
    public string DateOfBirth { get; set; }
    /// <summary>性别</summary>
    public string Gender { get; set; }
    /// <summary>家庭住址</summary>
    public string StreetAddress { get; set; }
    /// <summary>组织单元</summary>
    public StringIdNameEntity OrganizeUnit { get; set; }
}

/// <summary>用户搜索参数</summary>
public class UserSearchRequestModel : PaginatedRequestModel {

    /// <summary>用户名</summary>
    public string UserName { get; set; }
    /// <summary>排序方式</summary>
    public string SortBy { get; set; }
    /// <summary>角色名称</summary>
    public string RoleName { get; set; }
    /// <summary>组织单元ID</summary>
    public long? OrganizeUnitId { get; set; }
}

/// <summary>
/// 重置密码参数
/// </summary>
public class ResetPasswordModel {

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码必须填写！")]
    public string Password { get; set; }

    /// <summary>
    /// 密码确认
    /// </summary>
    [Compare("Password", ErrorMessage = "必须确认密码！")]
    public string ConfirmPassword { get; set; }

}
