using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>审计日志</summary>
    public partial class AppAuditLog : BaseEntity<long>  {

        /// <summary>请求路径</summary>
        public virtual string RequestPath { get; set; }
        /// <summary>请求方法</summary>
        public virtual string RequestMethod { get; set; }
        /// <summary>用户名</summary>
        public virtual string UserName { get; set; }
        /// <summary>开始时间</summary>
        public virtual DateTime StartAt { get; set; }
        /// <summary>耗时(毫秒)</summary>
        public virtual long Duration { get; set; }
        /// <summary>响应状态码</summary>
        public virtual int ResponseCode { get; set; }
        /// <summary>控制器名称</summary>
        public virtual string ControllerName { get; set; }
        /// <summary>动作名称</summary>
        public virtual string ActionName { get; set; }
        /// <summary>描述</summary>
        public virtual string Description { get; set; }

    }

}
