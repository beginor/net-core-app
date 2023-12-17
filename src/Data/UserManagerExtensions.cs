using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data;

public static class UserManagerExtensions {

    public static async Task<AppOrganizeUnit> GetUserOrganizeUnitAsync(
        this UserManager<AppUser> userManager,
        string userName,
        CancellationToken token = default
    ) {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null) {
            throw new InvalidOperationException($"Invalid user {userName}");
        }
        var unit = user.OrganizeUnit;
        if (unit == null) {
            throw new InvalidOperationException($"User {userName} does not have organize unit.");
        }
        return unit;
    }

}
