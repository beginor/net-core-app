using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Api.Controllers {

    partial class AccountController {

        [HttpGet("menu")]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<MenuNodeModel> GetMenuAsync() {
            try {
                List<string> roles;
                if (!User.Identity.IsAuthenticated || User.HasClaim(ClaimTypes.NameIdentifier, string.Empty)) {
                    roles = roleMgr.Roles
                        .Where(role => role.IsAnonymous == true)
                        .Select(role => role.Name)
                        .ToList();
                }
                else {
                    var user = await userMgr.FindByNameAsync(User.Identity.Name);
                    roles = (await userMgr.GetRolesAsync(user)).ToList();
                    roles.AddRange(
                        roleMgr.Roles.Where(role => role.IsDefault == true).Select(role => role.Name)
                    );
                }
                var menuModel = await navRepo.GetMenuAsync(roles.ToArray());
                return menuModel;
            }
            catch (Exception ex) {
                logger.LogError(ex, "Can not get menu!");
                return new MenuNodeModel();
            }
        }

    }

}
