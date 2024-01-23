using System;
using Beginor.AppFx.Core;

#nullable disable

namespace Beginor.NetCoreApp.Models;

/// <summary>附件表模型</summary>
public partial class AppAttachmentModel : StringEntity {
    /// <summary>内容类型（HTTP Content Type）</summary>
    public string ContentType { get; set; }
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>创建附件的用户Id</summary>
    public string CreatorId { get; set; }
    /// <summary>创建附件的用户名</summary>
    public string CreatorName { get; set; }
    /// <summary>文件名</summary>
    public string FileName { get; set; }
    /// <summary>附件大小</summary>
    public long Length { get; set; }
    /// <summary>附件所属的业务ID</summary>
    public string BusinessId { get; set; }
    /// <summary>文件路径</summary>
    public string FilePath { get; set; }
}

/// <summary>附件表搜索参数</summary>
public partial class AppAttachmentSearchModel : PaginatedRequestModel {

    /// <summary>业务ID</summary>
    public long? BusinessId { get; set; }

}

/// <summary>附件上传模型</summary>
public class AttachmentUploadModel {
    /// <summary>文件名</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>内容类型（HTTP Content Type）</summary>
    public string ContentType { get; set; } = string.Empty;
    /// <summary>附件大小</summary>
    public long Length { get; set; } = 0;
    /// <summary>附件所属的业务ID</summary>
    public string BusinessId { get; set; } = string.Empty;
    /// <summary>跳过长度</summary>
    public long Offset { get; set; } = 0;
    /// <summary>内容</summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();
}

/// <summary>附件上传结果模型</summary>
public class AttachmentUploadResultModel {
    /// <summary>文件名</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>附件大小</summary>
    public long Length { get; set; } = 0;
    /// <summary>已上传大小</summary>
    public long UploadedSize { get; set; } = 0;
}
