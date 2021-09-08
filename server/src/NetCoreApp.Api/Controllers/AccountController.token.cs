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
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    partial class AccountController {

        /// <summary>获取全部用户的访问凭证</summary>
        [HttpGet("tokens")]
        [Authorize()]
        public async Task<ActionResult<PaginatedResponseModel<AppUserTokenModel>>> Search(
            [FromQuery]AppUserTokenSearchModel model
        ) {
            try {
                var userId = this.GetUserId();
                var models = await userTokenRepo.SearchAsync(model, userId);
                return models;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get all tokens for user {User.Identity.Name} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>获取指定的用户凭证</summary>
        [HttpGet("tokens/{id:long}")]
        [Authorize]
        public async Task<ActionResult<AppUserTokenModel>> GetById([FromQuery]long id) {
            try {
                var model = await userTokenRepo.GetTokenForUserAsync(id, this.GetUserId());
                if (model == null) {
                    return NotFound();
                }
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get token {id} for user {User.Identity.Name} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>新建用户凭证</summary>
        [HttpPost("tokens")]
        [Authorize]
        public async Task<ActionResult<AppUserTokenModel>> Save(
            [FromBody]AppUserTokenModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(this.GetUserId());
                if (user == null) {
                    return BadRequest("Invalid user !");
                }
                await userTokenRepo.SaveTokenForUserAsync(model, user);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save token {model.ToJson()} to for user {User.Identity.Name} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>更新用户凭证</summary>
        [HttpPut("tokens/{id:long}")]
        [Authorize]
        public async Task<ActionResult<AppUserTokenModel>> Update(
            [FromRoute]long id,
            [FromBody]AppUserTokenModel model
        ) {
            try {
                var userId = this.GetUserId();
                var exists = await userTokenRepo.ExistsAsync(id, userId);
                if (!exists) {
                    return NotFound();
                }
                var user = await userMgr.FindByIdAsync(userId);
                if (user == null) {
                    return BadRequest("Invalid user !");
                }
                await userTokenRepo.UpdateTokenForUserAsync(id, model, user);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update token by id {id} with {model.ToJson()} for user {User.Identity.Name} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>删除用户凭证</summary>
        [HttpDelete("tokens/{id:long}")]
        [ProducesResponseType(204)]
        [Authorize]
        public async Task<ActionResult> Delete(long id) {
            try {
                await userTokenRepo.DeleteTokenForUserAsync(id, this.GetUserId());
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete app_user_tokens by id {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>生成新的凭证值</summary>
        [HttpPost("new-token-value")]
        [Authorize]
        public ActionResult<string> NewTokenValue() {
            return Guid.NewGuid().ToString("N");
        }
    }

}
