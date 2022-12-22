using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Beginor.GisHub.Api.Cors;

public class CorsPolicyProvider : ICorsPolicyProvider {

    private CorsPolicy policy;
    private CorsOptions options;

    public CorsPolicyProvider(
        IOptionsSnapshot<CorsPolicy> snapshot,
        IOptions<CorsOptions> options
    ) {
        if (snapshot == null) {
            throw new ArgumentNullException(nameof(snapshot));
        }
        if (options == null) {
            throw new ArgumentNullException(nameof(options));
        }
        this.policy = snapshot.Value;
        this.options = options.Value;
    }

    public Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName) {
        if (context == null) {
            throw new ArgumentNullException(nameof(context));
        }
        policyName ??= options.DefaultPolicyName;
        if (options.DefaultPolicyName.Equals(policyName, StringComparison.OrdinalIgnoreCase)) {
            return Task.FromResult<CorsPolicy?>(policy);
        }
        return Task.FromResult<CorsPolicy?>(null);
    }

}
