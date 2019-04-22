using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    public class AppAttachmentModel : StringEntity {

        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserId { get; set; }

    }

}
