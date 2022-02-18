using System;
using System.Collections.Generic;

namespace Beginor.GisHub.Gmap; 

public class EBusOptions {
    public string PaasId { get; set; }
    public string PaasToken { get; set; }
    public string GatewayUrl { get; set; }
    public Dictionary<string, string> Tiles { get; } = new (StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Services { get; } = new (StringComparer.OrdinalIgnoreCase);
}