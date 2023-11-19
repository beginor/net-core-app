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
    /// <summary>附件所属的业务ID，可以是任意表的ID，如果业务表有附件， 则需要根据业务表记录的ID，删除对应的附件。</summary>
    public string BusinessId { get; set; }

}

/// <summary>附件表搜索参数</summary>
public partial class AppAttachmentSearchModel : PaginatedRequestModel {

    /// <summary>业务ID</summary>
    public long? BusinessId { get; set; }

}
