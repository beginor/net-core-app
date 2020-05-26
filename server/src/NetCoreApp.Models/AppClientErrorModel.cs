using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>程序客户端错误记录模型</summary>
    public partial class AppClientErrorModel : StringEntity {

        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>错误发生时间</summary>
        [Required(ErrorMessage = "错误发生时间 必须填写！")]
        public DateTime OccuredAt { get; set; }
        /// <summary>用户浏览器代理</summary>
        [Required(ErrorMessage = "用户浏览器代理 必须填写！")]
        public string UserAgent { get; set; }
        /// <summary>错误发生的路径地址</summary>
        public string Path { get; set; }
        /// <summary>错误消息</summary>
        public string Message { get; set; }

    }

    /// <summary>程序客户端错误记录搜索参数</summary>
    public partial class AppClientErrorSearchModel : PaginatedRequestModel { }

}