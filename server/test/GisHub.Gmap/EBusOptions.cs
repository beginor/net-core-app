using System;
using System.Collections.Generic;

namespace Beginor.GisHub.Gmap;

public class EBusOptions {
    public string PaasId { get; set; } = string.Empty;
    public string PaasToken { get; set; } = string.Empty;
    public string GatewayUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Tiles { get; } = new (StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Services { get; } = new (StringComparer.OrdinalIgnoreCase);
}
