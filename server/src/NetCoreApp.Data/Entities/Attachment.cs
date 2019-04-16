using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    public partial class Attachment : Int64Entity {
        
        public virtual string ContentType { get; set; }
        public virtual byte[] Content { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string UserId { get; set; }
        
    }

}
