using System;
using System.Security.Claims;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Common;

public static class ClaimsPrincipalExtensions {

    public static long GetOrganizeUnitId(this ClaimsPrincipal user) {
        var claim = user.FindFirst(Consts.OrganizeUnitIdClaimType);
        if (claim == null) {
            throw new InvalidOperationException("The principal does not have organize unit claim.");
        }
        if (long.TryParse(claim.Value, out var organizeUnitId)) {
            return organizeUnitId;
        }
        throw new InvalidOperationException("The principal has invalid organize unit claim.");
    }

    public static string GetUserName(this ClaimsPrincipal user) {
        if (!user.Identity!.IsAuthenticated) {
            throw new InvalidOperationException("User is not authenticated!");
        }
        return user.Identity!.Name!;
    }

}
