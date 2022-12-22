using System;
using Microsoft.AspNetCore.Http;

namespace Beginor.GisHub.Common;

public static class HttpRequestExtensions {

    public static bool IsAjaxRequest(this HttpRequest request) {
        if (request.Headers.TryGetValue("X-Requested-With", out var value)) {
            return value.ToString().Contains(
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase
            );
        }
        return false;
    }
}
