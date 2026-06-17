using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Beginor.NetCoreApp.Api.Cors;

public class CorsPolicyProvider : ICorsPolicyProvider {

    private readonly CorsPolicy corsPolicy;
    private readonly CorsOptions corsOptions;

    public CorsPolicyProvider(
        IOptionsSnapshot<CorsPolicy> corsPolicySnapshot,
        IOptions<CorsOptions> corsOptions
    ) {
        if (corsPolicySnapshot == null) {
            throw new ArgumentNullException(nameof(corsPolicySnapshot));
        }
        if (corsOptions == null) {
            throw new ArgumentNullException(nameof(corsOptions));
        }
        this.corsPolicy = corsPolicySnapshot.Value;
        this.corsOptions = corsOptions.Value;
    }

    public Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName) {
        if (context == null) {
            throw new ArgumentNullException(nameof(context));
        }
        policyName ??= corsOptions.DefaultPolicyName;
        if (corsOptions.DefaultPolicyName.Equals(policyName, StringComparison.OrdinalIgnoreCase)) {
            return Task.FromResult<CorsPolicy?>(corsPolicy);
        }
        return Task.FromResult<CorsPolicy?>(null);
    }

}
