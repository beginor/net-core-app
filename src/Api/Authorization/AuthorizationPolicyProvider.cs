using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Beginor.AspNetCore.Authentication.Token;
using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Api.Authorization;

public class AuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> options
) : DefaultAuthorizationPolicyProvider(options) {

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null) {
            return policy;
        }
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireAuthenticatedUser()
            .RequireClaim(AppClaimTypes.Privilege, policyName)
            .AddAuthenticationSchemes(
                JwtBearerDefaults.AuthenticationScheme,
                TokenOptions.DefaultSchemaName
            );
        return builder.Build();
    }

}
