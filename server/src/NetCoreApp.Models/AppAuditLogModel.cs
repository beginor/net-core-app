using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>审计日志模型</summary>
    public partial class AppAuditLogModel : StringEntity {

        /// <summary>请求路径</summary>
        [Required(ErrorMessage = "请求路径 必须填写！")]
        public string RequestPath { get; set; }
        /// <summary>请求方法</summary>
        [Required(ErrorMessage = "请求方法 必须填写！")]
        public string RequestMethod { get; set; }
        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>开始时间</summary>
        [Required(ErrorMessage = "开始时间 必须填写！")]
        public DateTime StartAt { get; set; }
        /// <summary>耗时(毫秒)</summary>
        [Required(ErrorMessage = "耗时(毫秒) 必须填写！")]
        public long Duration { get; set; }
        /// <summary>响应状态码</summary>
        [Required(ErrorMessage = "响应状态码 必须填写！")]
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
        /// <summary>请求日期，精确到日</summary>
        public DateTime? RequestDate { get; set; } = DateTime.Today;
        /// <summary>用户名</summary>
        public string UserName { get; set; }
    }

}
