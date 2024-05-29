using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AspNetCore.Authentication.Token;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Authorization;

public class TokenEventsHandler {

    public async Task OnTokenReceived(TokenReceivedContext context) {
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
        if (token.User.LockoutEnabled && token.User.LockoutEnd > DateTimeOffset.Now) {
            context.Fail("User is locked out!");
        }
        if (token.Urls is { Length: > 0 }) {
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
        var claims = new List<Claim> {
            new (ClaimTypes.NameIdentifier, token.Value!),
            new (ClaimTypes.Name, $"{token.User!.UserName}:{token.Name}")
        };
        if (token.Privileges is { Length: > 0 }) {
            claims.AddRange(token.Privileges.Select(privilege => new Claim(AppClaimTypes.Privilege, privilege)));
        }
        if (token.Roles is { Length: > 0 }) {
            claims.AddRange(token.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }
        var identity = new ClaimsIdentity(claims, context.Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        context.Principal = principal;
        context.Success();
    }

}
