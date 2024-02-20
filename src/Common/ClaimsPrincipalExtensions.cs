using System;
using System.Linq;
using System.Security.Claims;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Common;

public static class ClaimsPrincipalExtensions {

    public static long GetOrganizeUnitId(this ClaimsPrincipal user) {
        if (!user.Identity!.IsAuthenticated) {
            throw new InvalidOperationException("User is not authenticated!");
        }
        var claim = user.FindFirst(AppClaimTypes.OrganizeUnitId);
        if (claim == null) {
            throw new InvalidOperationException("The principal does not have organize unit claim.");
        }
        if (long.TryParse(claim.Value, out var organizeUnitId)) {
            return organizeUnitId;
        }
        throw new InvalidOperationException("The principal has invalid organize unit claim.");
    }

    public static string GetOrganizeUnitCode(this ClaimsPrincipal user) {
        if (!user.Identity!.IsAuthenticated) {
            throw new InvalidOperationException("User is not authenticated!");
        }
        var claim = user.FindFirst(AppClaimTypes.OrganizeUnitCode);
        if (claim == null) {
            throw new InvalidOperationException("The principal does not have organize unit claim.");
        }
        return claim.Value;
    }

    public static string GetUserName(this ClaimsPrincipal user) {
        if (!user.Identity!.IsAuthenticated) {
            throw new InvalidOperationException("User is not authenticated!");
        }
        return user.Identity!.Name!;
    }

    public static (long id, string code) GetOrganizeUnitIdAndCode(this ClaimsPrincipal user) {
        var id = user.GetOrganizeUnitId();
        var code = user.GetOrganizeUnitCode();
        return (id, code);
    }

    public static string[] GetRoles(this ClaimsPrincipal user) {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
    }

    public static bool IsInRole(this ClaimsPrincipal user, string role) {
        var claim = user.FindFirst(c => c.Type == ClaimTypes.Role && c.Value.EqualsOrdinalIgnoreCase(role));
        return claim != null;
    }

}
