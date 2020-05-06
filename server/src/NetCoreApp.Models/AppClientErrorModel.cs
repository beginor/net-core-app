using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>客户端错误模型</summary>
    public class AppClientErrorModel : StringEntity {

        /// <summary>用户名</summary>
        public virtual string UserName { get; set; }

        /// <summary>错误发生时间</summary>
        public virtual DateTime OccuredAt { get; set; }

        /// <summary>用户浏览器代理</summary>
        public virtual string UserAgent { get; set; }

        /// <summary>错误发生的路径地址</summary>
        public virtual string Path { get; set; }

        /// <summary>错误消息</summary>
        public virtual string Message { get; set; }
    }

    /// <summary>客户端错误搜索模型</summary>
    public partial class AppClientErrorSearchModel : PaginatedRequestModel { }

}
