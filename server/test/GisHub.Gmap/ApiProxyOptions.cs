using System;
using System.Collections.Generic;

namespace Gmap {

    public class ApiProxyOptions {
        public string PaasId { get; set; }
        public string PaasToken { get; set; }
        public string GatewayUrl { get; set; }
        public Dictionary<string, string> Services { get; } = new (StringComparer.OrdinalIgnoreCase);
    }
}
