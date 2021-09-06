using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Beginor.AspNetCore.Authentication.Token;
using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Api.Authorization {

    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider {

        public AuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options
        ) : base(options) { }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName) {
            var builder = new AuthorizationPolicyBuilder();
            builder.RequireAuthenticatedUser()
                .RequireClaim(Consts.PrivilegeClaimType, policyName)
                .AddAuthenticationSchemes(
                    JwtBearerDefaults.AuthenticationScheme,
                    TokenOptions.DefaultSchemaName
                );
            return Task.FromResult(builder.Build());
        }

    }

}
