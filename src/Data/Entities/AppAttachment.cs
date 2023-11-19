using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

#nullable disable

namespace Beginor.NetCoreApp.Data.Entities;

/// <summary>附件表</summary>
[Class(Schema = "public", Table = "app_attachments")]
public partial class AppAttachment : BaseEntity<long> {

    /// <summary>附件表ID</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }
    /// <summary>内容类型（HTTP Content Type）</summary>
    [Property(Name = "ContentType", Column = "content_type", Type = "string", NotNull = true, Length = 64)]
    public virtual string ContentType { get; set; }
    /// <summary>创建时间</summary>
    [Property(Name = "CreatedAt", Column = "created_at", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = true)]
    public virtual DateTime CreatedAt { get; set; }
    /// <summary>创建附件的用户</summary>
    [ManyToOne(Name = "Creator", Column = "creator_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser Creator { get; set; }
    /// <summary>文件名</summary>
    [Property(Name = "FileName", Column = "file_name", Type = "string", NotNull = true, Length = 256)]
    public virtual string FileName { get; set; }
    /// <summary>附件大小</summary>
    [Property(Name = "Length", Column = "length", Type = "long", NotNull = true)]
    public virtual long Length { get; set; }
    /// <summary>附件所属的业务ID，可以是任意表的ID，如果业务表有附件， 则需要根据业务表记录的ID，删除对应的附件。</summary>
    [Property(Name = "BusinessId", Column = "business_id", Type = "long", NotNull = true)]
    public virtual long BusinessId { get; set; }

}
