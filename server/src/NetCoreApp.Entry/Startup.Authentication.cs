using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Beginor.AppFx.Core;
using Beginor.AspNetCore.Authentication.Token;
using Beginor.NetCoreApp.Api.Authorization;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureAuthenticationServices(IServiceCollection services, IWebHostEnvironment env) {
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
                IssuerSigningKey = new SymmetricSecurityKey(jwt!.SecretKey),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            x.Events = new JwtBearerEvents {
                OnTokenValidated = async context => {
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
            };
        }).AddToken(options => {
            options.Events = new TokenEvents {
                OnTokenReceived = async context => {
                    var repo = context.HttpContext.RequestServices.GetService<IAppUserTokenRepository>();
                    var token = await repo!.GetTokenByValueAsync(context.Token!);
                    if (token == null) {
                        context.Fail("Invalid token!");
                        return;
                    }
                    if (token.ExpiresAt != null && token.ExpiresAt.Value < DateTime.Now) {
                        context.Fail("Token expires!");
                        return;
                    }
                    if (token.Urls != null && token.Urls.Length > 0) {
                        var req = context.Request;
                        string referer = req.Headers.Referer!;
                        if (referer.IsNullOrEmpty()) {
                            context.Fail("No referer provided");
                            return;
                        }
                        var isValid = token.Urls.Any(url => referer.StartsWith(url));
                        if (!isValid) {
                            context.Fail("Invalid referer!");
                            return;
                        }
                    }
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, token.Value!),
                        new Claim(ClaimTypes.Name, $"{token.User!.UserName}:{token.Name}")
                    };
                    if (token.Privileges != null && token.Privileges.Length > 0) {
                        foreach (var privilege in token.Privileges) {
                            var claim = new Claim(Consts.PrivilegeClaimType, privilege);
                            claims.Add(claim);
                        }
                    }
                    if (token.Roles != null && token.Roles.Length > 0) {
                        foreach (var role in token.Roles) {
                            var claim = new Claim(ClaimTypes.Role, role);
                            claims.Add(claim);
                        }
                    }
                    var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    context.Principal = principal;
                    context.Success();
                    return;
                }
            };
        });
        // authorization;
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
    }

    private void ConfigureAuthentication(WebApplication app, IWebHostEnvironment env) {
        app.UseAuthentication();
        app.UseAuthorization();
    }

}
