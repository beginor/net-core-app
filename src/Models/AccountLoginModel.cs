using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

#nullable disable

namespace Beginor.NetCoreApp.Models;

/// <summary>登录信息</summary>
public class AccountLoginModel {

    /// <summary>用户名</summary>
    [Required(ErrorMessage = "用户名必须填写！")]
    public string UserName { get; set; }

    /// <summary>密码</summary>
    [Required(ErrorMessage = "密码必须填写！")]
    public string Password { get; set; }

    /// <summary>保持登录</summary>
    public bool IsPersistent { get; set; }

    /// <summary>验证码</summary>
    [Required(ErrorMessage = "验证码必须填写！")]
    public string Captcha { get; set; }
}

/// <summary>已经登录的账户信息</summary>
public class AccountInfoModel : StringEntity {

    /// <summary>用户名</summary>
    public string UserName { get; set; }

    /// <summary>名字</summary>
    public string GivenName { get; set; }

    /// <summary>姓氏</summary>
    public string Surname { get; set; }

    /// <summary>角色</summary>
    public IDictionary<string, bool> Roles { get; set; }

    /// <summary>权限</summary>
    public IDictionary<string, bool> Privileges { get; set; }
    /// <summary>JwtToken</summary>
    public string Token { get; set; }
}

/// <summary>用户看到的菜单项</summary>
public class MenuNodeModel {
    /// <summary>菜单ID</summary>
    public string Id { get; set; }
    /// <summary>菜单地址 Url</summary>
    public string Url { get; set; }
    /// <summary>菜单标题</summary>
    public string Title { get; set; }
    /// <summary>菜单图标</summary>
    public string Icon { get; set; }
    /// <summary>菜单提示文字</summary>
    public string Tooltip { get; set; }
    /// <summary>自菜单项</summary>
    public MenuNodeModel[] Children { get; set; }
    /// <summary>目标窗口</summary>
    public string Target { get; set; }
    /** 内嵌窗口地址 */
    public string FrameUrl { get; set; }
    /** 是否隐藏 */
    public bool IsHidden { get; set; }
}

/// <summary>用户修改密码</summary>
public class ChangePasswordModel {
    /// <summary>当前密码</summary>
    [Required(ErrorMessage = "当前密码必须填写！")]
    public string CurrentPassword { get; set; }
    /// <summary>新密码</summary>
    [Required(ErrorMessage = "新密码必须填写！")]
    public string NewPassword { get; set; }
    /// <summary>确认新密码</summary>
    [Compare("NewPassword", ErrorMessage = "必须确认新密码！")]
    public string ConfirmPassword { get; set; }
}
