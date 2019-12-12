using System;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Beginor.NetCoreApp.Api.Authorization;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureAuthenticationServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var jwt = config.GetSection("jwt").Get<Jwt>();
            services.AddSingleton(jwt);
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwt.SecretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            // authorization;
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }

        private void ConfigureAuthentication(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseAuthentication();
            app.UseAuthorization();
        }

    }

    public class Jwt {
        public string Secret { get; set; }
        public TimeSpan ExpireTimeSpan { get; set; }
        public byte[] SecretKey {
            get {
                if (string.IsNullOrEmpty(Secret)) {
                    throw new InvalidOperationException("Secret is empty!");
                }
                return Encoding.ASCII.GetBytes(Secret);
            }
        }
    }

}
