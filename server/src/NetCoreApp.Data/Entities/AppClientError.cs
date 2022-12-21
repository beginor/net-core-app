using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Data.Entities;

#nullable disable

/// <summary>程序客户端错误记录</summary>
[Class(Schema = "public", Table = "app_client_errors")]
public partial class AppClientError : BaseEntity<long> {

    /// <summary>客户端记录ID</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>用户名</summary>
    [Property(Name = "UserName", Column = "user_name", Type = "string", NotNull = false, Length = 64)]
    public virtual string UserName { get; set; }

    /// <summary>错误发生时间</summary>
    [Property(Name = "OccuredAt", Column = "occured_at", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampTzType), NotNull = true)]
    public virtual DateTime OccuredAt { get; set; }

    /// <summary>用户浏览器代理</summary>
    [Property(Name = "UserAgent", Column = "user_agent", Type = "string", NotNull = true, Length = 512)]
    public virtual string UserAgent { get; set; }

    /// <summary>错误发生的路径地址</summary>
    [Property(Name = "Path", Column = "path", Type = "string", NotNull = false, Length = 1024)]
    public virtual string Path { get; set; }

    /// <summary>错误消息</summary>
    [Property(Name = "Message", Column = "message", Type = "string", NotNull = false, Length = 1024)]
    public virtual string Message { get; set; }
}
