using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>审计日志</summary>
    [Class(Schema = "public", Table = "app_audit_logs")]
    public partial class AppAuditLog : BaseEntity<long> {

        /// <summary>审计日志ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>客户端 IP 地址</summary>
        [Property(Name = "Ip", Column = "ip", Type = "string", NotNull = false, Length = 64)]
        public virtual string Ip { get; set; }

        /// <summary>请求路径</summary>
        [Property(Name = "RequestPath", Column = "request_path", Type = "string", NotNull = true, Length = 256)]
        public virtual string RequestPath { get; set; }
        /// <summary>请求方法</summary>
        [Property(Name = "RequestMethod", Column = "request_method", Type = "string", NotNull = true, Length = 8)]
        public virtual string RequestMethod { get; set; }
        /// <summary>用户名</summary>
        [Property(Name = "UserName", Column = "user_name", Type = "string", NotNull = true, Length = 64)]
        public virtual string UserName { get; set; }
        /// <summary>开始时间</summary>
        [Property(Name = "StartAt", Column = "start_at", Type = "datetime", NotNull = true)]
        public virtual DateTime StartAt { get; set; }
        /// <summary>耗时(毫秒)</summary>
        [Property(Name = "Duration", Column = "duration", Type = "long", NotNull = true)]
        public virtual long Duration { get; set; }
        /// <summary>响应状态码</summary>
        [Property(Name = "ResponseCode", Column = "response_code", Type = "int", NotNull = true)]
        public virtual int ResponseCode { get; set; }
        /// <summary>控制器名称</summary>
        [Property(Name = "ControllerName", Column = "controller_name", Type = "string", NotNull = false, Length = 64)]
        public virtual string ControllerName { get; set; }
        /// <summary>动作名称</summary>
        [Property(Name = "ActionName", Column = "action_name", Type = "string", NotNull = false, Length = 64)]
        public virtual string ActionName { get; set; }
        /// <summary>描述</summary>
        [Property(Name = "Description", Column = "description", Type = "string", NotNull = false, Length = 256)]
        public virtual string Description { get; set; }

    }

}
