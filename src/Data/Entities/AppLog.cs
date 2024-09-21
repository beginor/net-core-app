using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

#nullable disable

namespace Beginor.NetCoreApp.Data.Entities;

/// <summary>应用程序日志</summary>
[Class(Schema = "public", Table = "app_logs")]
public partial class AppLog : BaseEntity<long> {
    /// <summary>日志ID</summary>
    [Id(Name = nameof(Id), Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }
    /// <summary>创建时间</summary>
    [Property(Name = nameof(CreatedAt), Column = "created_at", Type = "timestamp", NotNull = true)]
    public virtual DateTime CreatedAt { get; set; }
    /// <summary>线程ID</summary>
    [Property(Name = nameof(Thread), Column = "thread", Type = "string", NotNull = true, Length = 8)]
    public virtual string Thread { get; set; }
    /// <summary>日志级别</summary>
    [Property(Name = nameof(Level), Column = "level", Type = "string", NotNull = true, Length = 16)]
    public virtual string Level { get; set; }
    /// <summary>记录者</summary>
    [Property(Name = nameof(Logger), Column = "logger", Type = "string", NotNull = false, Length = 256)]
    public virtual string Logger { get; set; }
    /// <summary>日志消息</summary>
    [Property(Name = nameof(Message), Column = "message", Type = "string", NotNull = false, Length = 4096)]
    public virtual string Message { get; set; }
    /// <summary>异常信息</summary>
    [Property(Name = nameof(Exception), Column = "exception", Type = "string", NotNull = false, Length = 4096)]
    public virtual string Exception { get; set; }
}
