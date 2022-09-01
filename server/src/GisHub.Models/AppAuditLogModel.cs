using System;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models; 

/// <summary>审计日志模型</summary>
public partial class AppAuditLogModel : StringEntity {

    /// <summary>客户端 IP 地址</summary>
    public string Ip { get; set; }
    /// <summary>请求的主机名</summary>
    public string HostName { get; set; }
    /// <summary>请求路径</summary>
    public string RequestPath { get; set; }
    /// <summary>请求方法</summary>
    public string RequestMethod { get; set; }
    /// <summary>用户名</summary>
    public string UserName { get; set; }
    /// <summary>开始时间</summary>
    public DateTime StartAt { get; set; }
    /// <summary>耗时(毫秒)</summary>
    public long Duration { get; set; }
    /// <summary>响应状态码</summary>
    public int ResponseCode { get; set; }
    /// <summary>控制器名称</summary>
    public string ControllerName { get; set; }
    /// <summary>动作名称</summary>
    public string ActionName { get; set; }
    /// <summary>描述</summary>
    public string Description { get; set; }

}

/// <summary>审计日志搜索参数</summary>
public partial class AppAuditLogSearchModel : PaginatedRequestModel {
    /// <summary>请求开始日期，精确到日</summary>
    public DateTime? StartDate { get; set; }
    /// <summary>请求结束日期，精确到日</summary>
    public DateTime? EndDate { get; set; }
    /// <summary>用户名</summary>
    public string UserName { get; set; }
}

/// <summary>审计日志访问量</summary>
public class AppAuditLogTrafficStatModel {
    /// <summary>请求日期</summary>
    public DateTime RequestDate { get; set; }
    /// <summary>请求数量</summary>
    public long RequestCount { get; set; }
    /// <summary>平均响应时间</summary>
    public double AvgDuration { get; set; }
    /// <summary>最大响应时间</summary>
    public double MaxDuration { get; set; }
    /// <summary>最小响应设时间</summary>
    public double MinDuration { get; set; }
}

/// <summary>访问日志状态码统计</summary>
public class AppAuditLogStatusStatModel {
    /// <summary>状态码</summary>
    public int StatusCode { get; set; }
    /// <summary>请求数量</summary>
    public int RequestCount { get; set; }
}

/// <summary>访问日志响应时间统计</summary>
public class AppAuditLogDurationStatModel {
    /// <summary>响应时间</summary>
    public string Duration { get; set; }
    /// <summary>请求数量</summary>
    public int RequestCount { get; set; }
}

/// <summary>访问日志响应时间统计</summary>
public class AppAuditLogUserStatModel {
    /// <summary>用户名</summary>
    public string Username { get; set; }
    /// <summary>请求数量</summary>
    public int RequestCount { get; set; }
}

/// <summary>访问日志响应时间统计</summary>
public class AppAuditLogIpStatModel {
    /// <summary>用户IP地址</summary>
    public string Ip { get; set; }
    /// <summary>请求数量</summary>
    public int RequestCount { get; set; }
}
