using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models; 

/// <summary>应用程序角色模型</summary>
public class AppRoleModel : StringEntity {

    /// <summary>角色名称</summary>
    [Required(ErrorMessage = "角色名称必须填写")]
    public string Name { get; set; }

    /// <summary>角色描述</summary>
    public string Description { get; set; }

    /// <summary>是否默认角色</summary>
    public bool IsDefault { get; set; }

    /// <summary>是否匿名角色</summary>
    public bool IsAnonymous { get; set; }

    /// <summary>用户数</summary>
    public int UserCount { get; set; }

}

/// <summary>角色搜索参数</summary>
public class AppRoleSearchModel : PaginatedRequestModel {

    /// <summary>角色名称</summary>
    public string Name { get; set; }
}

/// <summary>角色权限模型</summary>
public class AppRoleWithPrivilegesModel {
    /// <summary>角色ID</summary>
    public string Id { get; set; }
    /// <summary>角色名称</summary>
    public string Name { get; set; }
    /// <summary>角色描述</summary>
    public string Description { get; set; }
    /// <summary>角色权限</summary>
    public string[] Privileges { get; set; }
}