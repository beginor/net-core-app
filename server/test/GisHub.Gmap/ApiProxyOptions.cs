using System;
using System.Collections.Generic;
using System.Linq;

namespace Beginor.GisHub.Gmap; 

public class ApiProxyOptions {
    public string PaasId { get; set; }
    public string PaasToken { get; set; }
    public string GatewayUrl { get; set; }
    public List<ApiProxyService> Services { get; } = new ();

    public void CheckServiceConfig() {
        foreach (var service in Services) {
            if (string.IsNullOrEmpty(service.PaasId)) {
                service.PaasId = PaasId;
            }
            if (string.IsNullOrEmpty(service.PaasToken)) {
                service.PaasToken = PaasToken;
            }
            if (string.IsNullOrEmpty(service.GatewayUrl)) {
                service.GatewayUrl = GatewayUrl;
            }
        }
    }

    public ApiProxyService FindServiceById(string serviceId) {
        var svc = Services.FirstOrDefault(s => serviceId.Equals(s.Id, StringComparison.OrdinalIgnoreCase));
        if (svc != null) {
            if (string.IsNullOrEmpty(svc.PaasId)) {
                svc.PaasId = PaasId;
            }
            if (string.IsNullOrEmpty(svc.PaasToken)) {
                svc.PaasToken = PaasToken;
            }
            if (string.IsNullOrEmpty(svc.GatewayUrl)) {
                svc.GatewayUrl = GatewayUrl;
            }
        }
        return svc;
    }
}

public class ApiProxyService {
    public string Id { get; set; }
    public string Name { get; set; }
    public string PaasId { get; set; }
    public string PaasToken { get; set; }
    public string GatewayUrl { get; set; }
    public string TileTemplate { get; set; }
}