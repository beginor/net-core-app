using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Beginor.AspNetCore.Authentication.Token;
using Beginor.NetCoreApp.Api.Authorization;
using Beginor.NetCoreApp.Common;
using Gdeei.Impress.Api.Authorization;

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
            var handler = new JwtBearerEventsHandler();
            x.Events = new JwtBearerEvents {
                OnTokenValidated = handler.OnTokenValidated
            };
        }).AddToken(options => {
            var handler = new TokenEventsHandler();
            options.Events = new TokenEvents {
                OnTokenReceived = handler.OnTokenReceived
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
