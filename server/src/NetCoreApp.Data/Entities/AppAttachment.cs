using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>附件表</summary>
    public partial class AppAttachment : BaseEntity<long>  {

            /// <summary>内容类型（HTTP Content Type）</summary>
        public virtual string ContentType { get; set; }
        /// <summary>附件内容</summary>
        public virtual byte[] Content { get; set; }
        /// <summary>创建时间</summary>
        public virtual DateTime CreatedAt { get; set; }
        /// <summary>创建附件的用户ID</summary>
        public virtual string CreatorId { get; set; }
        /// <summary>文件名</summary>
        public virtual string FileName { get; set; }
        /// <summary>附件大小</summary>
        public virtual long Length { get; set; }
        /// <summary>附件所属的业务ID，可以是任意表的ID，如果业务表有附件， 则需要根据业务表记录的ID，删除对应的附件。</summary>
        public virtual long BusinessId { get; set; }

    }

}
