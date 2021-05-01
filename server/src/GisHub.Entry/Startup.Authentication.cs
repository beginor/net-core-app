using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Beginor.AppFx.Core;
using Beginor.GisHub.Api.Authorization;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureAuthenticationServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var section = config.GetSection("jwt");
            services.Configure<JwtOption>(section);
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                var jwt = section.Get<JwtOption>();
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwt.SecretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents {
                    OnTokenValidated = async context => {
                        var authCache = context.HttpContext.RequestServices.GetService<IAuthorizationCache>();
                        var identity = context.Principal.Identity as ClaimsIdentity;
                        var userId = identity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                        if (userId.IsNullOrEmpty()) {
                            userId = "anonymous";
                        }
                        var cachedClaims = await authCache.GetUserClaimsAsync(userId);
                        foreach (var claim in cachedClaims) {
                            identity.AddClaim(claim);
                        }
                    }
                };
            });
            // authorization;
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationCache, AuthorizationCache>();
        }

        private void ConfigureAuthentication(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseAuthentication();
            app.UseAuthorization();
        }

    }

}
