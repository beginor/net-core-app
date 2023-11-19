using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

#nullable disable

namespace Beginor.NetCoreApp.Models;

/// <summary>应用存储模型</summary>
public partial class AppStorageModel : StringEntity {

    /// <summary>应用存储别名</summary>
    [Required(ErrorMessage = "存储别名 必须填写！")]
    public string AliasName { get; set; }
    /// <summary>应用存储路径</summary>
    [Required(ErrorMessage = "应用存储根路径 必须填写！")]
    public string RootFolder { get; set; }
    /// <summary>是否只读</summary>
    [Required(ErrorMessage = "是否只读 必须填写！")]
    public bool Readonly { get; set; }
    /// <summary>可访问此存储的角色</summary>
    public string[] Roles { get; set; }

}

/// <summary>应用存储搜索参数</summary>
public partial class AppStorageSearchModel : PaginatedRequestModel {
    /// <summary>搜索关键字</summary>
    public string Keywords { get; set; }
}

/// <summary>服务目录浏览模型</summary>
public class AppStorageBrowseModel {
    /// <summary>别名</summary>
    public string Alias { get; set; }
    /// <summary>请求路径 (/bi/)</summary>
    public string Path { get; set; }
    /// <summary>文件过滤 (*.*)</summary>
    public string Filter { get; set; }
    /// <summary>子目录名称</summary>
    public string[] Folders { get; set; }
    /// <summary>文件名</summary>
    public string[] Files { get; set; }
}
