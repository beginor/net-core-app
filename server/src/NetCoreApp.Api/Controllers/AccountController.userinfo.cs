using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NHibernate.Linq;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Api.Authorization;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    partial class AccountController {

        /// <summary>获取当前用户信息</summary>
        [HttpGet("user")] // GET /api/account/user
        [Authorize]
        public async Task<ActionResult<AppUserModel>> GetUser() {
            var userIdStr = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (userIdStr.IsNullOrEmpty()) {
                return Forbid();
            }
            if (!long.TryParse(userIdStr, out var userId)) {
                return Forbid();
            }
            return await usersCtrl.GetById(userId);
        }

        // PUT /api/account/user;
        /// <summary>更新当前用户信息</summary>
        [HttpPut("user")]
        [Authorize]
        public async Task<ActionResult<AppUserModel>> UpdateUser([FromBody]AppUserModel model) {
            var userIdStr = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (userIdStr.IsNullOrEmpty()) {
                return Forbid();
            }
            if (!long.TryParse(userIdStr, out var userId)) {
                return Forbid();
            }
            return await usersCtrl.Update(userId, model);
        }
    }

}
