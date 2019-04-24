using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>附件信息</summary>
    public class AppAttachmentModel : StringEntity {

        /// <summary>附件类型</summary>
        public string ContentType { get; set; }
        /// <summary>附件内容</summary>
        public byte[] Content { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }
        /// <summary>用户ID</summary>
        public string UserId { get; set; }

    }

    /// <summary>附件搜索参数</summary>
    public class AppAttachmentSearchModel : PaginatedRequestModel {

        /// <summary>用户ID</summary>
        public string UserId { get; set; }

        /// <summary>文件类型</summary>
        public string ContentType { get; set; }

    }

}
