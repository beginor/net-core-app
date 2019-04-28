using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    public partial class AppAttachment : Int64Entity {

        public virtual string ContentType { get; set; }
        public virtual byte[] Content { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string CreatorId { get; set; }
        public virtual string FileName { get; set; }
        public virtual long Length { get; set; }
        public virtual long BusinessId { get; set; }

    }

}
