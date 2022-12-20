using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Models;

/// <summary>数据源模型</summary>
public partial class DataSourceModel : StringEntity {

    /// <summary>数据源名称</summary>
    [Required(ErrorMessage = "数据源名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>数据库类型（postgres、mssql、mysql、oracle、sqlite等）</summary>
    [Required(ErrorMessage = "数据库类型（postgres、mssql、mysql、oracle、sqlite等） 必须填写！")]
    public string DatabaseType { get; set; }
    /// <summary> 服务器地址 </summary>
    [Required(ErrorMessage = "服务器地址 必须填写！")]
    public string ServerAddress { get; set; }
    /// <summary> 服务器端口 </summary>
    public int ServerPort { get; set; }
    /// <summary> 数据库名称 </summary>
    [Required(ErrorMessage = "数据库名称 必须填写！")]
    public string DatabaseName { get; set; }
    /// <summary> 用户名 </summary>
    public string Username { get; set; }
    /// <summary> 密码 </summary>
    public string Password { get; set; }
    /// <summary> 超时时间（秒） </summary>
    public int Timeout { get; set; }
    /// <summary>使用 ssl 安全连接</summary>
    public virtual bool UseSsl { get; set; }

}

/// <summary>数据源搜索参数</summary>
public partial class DataSourceSearchModel : PaginatedRequestModel {
    public string? Keywords { get; set; }
}
