using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Api.Authorization;

public class JwtBearerEventsHandler {

    public async Task OnTokenValidated(TokenValidatedContext context) {
        var cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
        var identity = context.Principal!.Identity as ClaimsIdentity;
        var userId = identity!.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        if (userId.IsNullOrEmpty()) {
            userId = "anonymous";
        }
        var cachedClaims = await cache!.GetUserClaimsAsync(userId);
        foreach (var claim in cachedClaims) {
            identity.AddClaim(claim);
        }
    }

}
