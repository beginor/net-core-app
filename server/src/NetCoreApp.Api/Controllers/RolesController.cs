using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>角色 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private RoleManager<AppRole> roleMgr;
        private UserManager<AppUser> userMgr;

        public RolesController(
            RoleManager<AppRole> roleMgr,
            UserManager<AppUser> userMgr
        ) {
            this.roleMgr = roleMgr;
            this.userMgr = userMgr;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                roleMgr = null;
                userMgr = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>创建角色</summary>
        /// <response code="200">创建角色成功并返回角色信息</response>
        /// <response code="400">创建角色失败并返回错误信息</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult<AppRoleModel>> Create(
            [FromBody]AppRoleModel model
        ) {
            try {
                if (await roleMgr.RoleExistsAsync(model.Name)) {
                    return BadRequest($"Role already {model.Name} exists!");
                }
                var role = Mapper.Map<AppRole>(model);
                var result = await roleMgr.CreateAsync(role);
                if (result.Succeeded) {
                    Mapper.Map(role, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not create role", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除指定的角色</summary>
        /// <response code="204">删除角色成功</response>
        /// <response code="400">删除角色出错并返回错误信息</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete(string id) {
            try {
                var role = await roleMgr.FindByIdAsync(id);
                if (role == null) {
                    return NoContent();
                }
                var result = await roleMgr.DeleteAsync(role);
                if (result.Succeeded) {
                    return NoContent();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not delete role", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取角色列表</summary>
        /// <response code="200">获取成功并返回角色列表。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpGet("")]
        public async Task<ActionResult<PaginatedResponseModel<AppRoleModel>>> GetAll(
            [FromQuery]RoleSearchRequestModel model
        ) {
            try {
                var query = roleMgr.Roles;
                if (model.Name.IsNotNullOrEmpty()) {
                    query = query.Where(r => r.Name.Contains(model.Name));
                }
                var total = await query.LongCountAsync();
                var roles = await query.ToListAsync();
                var models = Mapper.Map<IList<AppRoleModel>>(roles);
                var result = new PaginatedResponseModel<AppRoleModel> {
                    Skip = model.Skip,
                    Take = model.Take,
                    Total = total,
                    Data = models
                };
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all roles!", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取指定角色</summary>
        /// <response code="404">找不到指定的角色。</response>
        /// <response code="200">获取角色成功，返回角色信息。</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppRoleModel>> GetById(
            string id
        ) {
            try {
                var role = await roleMgr.FindByIdAsync(id);
                if (role == null) {
                    return NotFound();
                }
                var model = Mapper.Map<AppRoleModel>(role);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get role by id {id}", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>更新指定的角色</summary>
        /// <response code="200">更新成功并返回角色信息</response>
        /// <response code="400">更新角色出错并返回错误信息</response>
        /// <response code="404">指定的角色不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        public async Task<ActionResult<AppRoleModel>> Update(
            [FromRoute]string id,
            [FromBody]AppRoleModel model
        ) {
            try {
                var role = await roleMgr.FindByIdAsync(id);
                if (role == null) {
                    return NotFound();
                }
                Mapper.Map(model, role);
                var result = await roleMgr.UpdateAsync(role);
                if (result.Succeeded) {
                    Mapper.Map(role, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not update role", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取指定角色下的的用户</summary>
        /// <response code="200">获取成功，返回该角色的用户列表</response>
        /// <response code="404">指定的角色不存在</response>
        /// <response code=""></response>
        [HttpGet("{id:long}/users")]
        public async Task<ActionResult<IList<AppUserModel>>> GetUsersInRole(
            string id
        ) {
            try {
                var role = await roleMgr.FindByIdAsync(id);
                if (role == null) {
                    return NotFound();
                }
                var users = await userMgr.GetUsersInRoleAsync(role.Name);
                var models = Mapper.Map<IList<AppUserModel>>(users);
                return models.ToList();
            }
            catch (Exception ex) {
                logger.Error($"Can not get users in role", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
