using System;
using System.Collections.Generic;

namespace Gmap {

    public class EBusOptions {
        public string PaasId { get; set; }
        public string PaasToken { get; set; }
        public string GatewayUrl { get; set; }
        public Dictionary<string, string> Tiles { get; }
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

}
