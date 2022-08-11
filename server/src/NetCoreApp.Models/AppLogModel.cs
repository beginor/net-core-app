using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models;

/// <summary>应用程序日志模型</summary>
public partial class AppLogModel : StringEntity {
    /// <summary>线程ID</summary>
    public string Thread { get; set; }
    /// <summary>日志级别</summary>
    public string Level { get; set; }
    /// <summary>记录者</summary>
    public string Logger { get; set; }
    /// <summary>日志消息</summary>
    public string Message { get; set; }
    /// <summary>异常信息</summary>
    public string Exception { get; set; }
}

/// <summary>应用程序日志搜索参数</summary>
public partial class AppLogSearchModel : PaginatedRequestModel {
    /// <summary>开始日期</summary>
    public DateTime? StartDate { get; set; }
    /// <summary>结束日期</summary>
    public DateTime? EndDate { get; set; }
    /// <summary>日志级别</summary>
    public string Level { get; set; }
}
