using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>用户 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private UserManager<ApplicationUser> manager;

        public UsersController(UserManager<ApplicationUser> manager) {
            this.manager = manager;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                manager = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>创建用户</summary>
        /// <response code="200">创建用户成功</response>
        /// <response code="400">创建用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult<ApplicationUserModel>> Create(
            [FromBody]ApplicationUserModel model
        ) {
            try {
                var user = await manager.FindByNameAsync(model.UserName);
                if (user != null) {
                    return BadRequest($"User with {model.UserName} exists!");
                }
                user = await manager.FindByEmailAsync(model.Email);
                if (user != null) {
                    return BadRequest($"User with {model.Email} exists!");
                }
                user = Mapper.Map<ApplicationUser>(model);
                var result = await manager.CreateAsync(user);
                if (result.Succeeded) {
                    Mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not create user", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>删除指定的用户</summary>
        /// <response code="204">删除用户成功</response>
        /// <response code="400">删除用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id) {
            try {
                var user = await manager.FindByIdAsync(id);
                if (user == null) {
                    return NoContent();
                }
                var result = await manager.DeleteAsync(user);
                if (result.Succeeded) {
                    return NoContent();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not delete user", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>获取用户列表</summary>
        /// <response code="200">获取成功并返回用户列表。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpGet("")]
        public async Task<ActionResult<PaginatedResponseModel<ApplicationUserModel>>> GetAll(
            [FromQuery]UserSearchRequestModel param
        ) {
            try {
                var query = manager.Users;
                if (param.UserName.IsNotNullOrEmpty()) {
                    query = query.Where(u => u.UserName.Contains(param.UserName));
                }
                var total = query.Count();
                var data = await query.ToListAsync();
                var models = Mapper.Map<IList<ApplicationUserModel>>(data);
                var result = new PaginatedResponseModel<ApplicationUserModel> {
                    Skip = param.Skip,
                    Take = param.Take,
                    Total = total,
                    Data = models
                };
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get all user", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>获取指定用户</summary>
        /// <response code="404">找不到指定的用户。</response>
        /// <response code="200">获取用户成功，返回用户信息。</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserModel>> GetById(string id) {
            try {
                var user = await manager.FindByIdAsync(id);
                if (user == null) {
                    return NotFound();
                }
                var model = Mapper.Map<ApplicationUserModel>(user);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get user with id {id}", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>更新指定的用户</summary>
        /// <response code="200">更新成功并返回用户信息</response>
        /// <response code="400">更新用户出错并返回用户信息</response>
        /// <response code="404">指定的用户不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationUserModel>> Update(
            [FromRoute]string id,
            [FromBody]ApplicationUserModel model
        ) {
            try {
                var user = await manager.FindByIdAsync(id);
                if (user == null) {
                    return NotFound();
                }
                Mapper.Map(model, user);
                var result = await manager.UpdateAsync(user);
                if (result.Succeeded) {
                    Mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not update user with id {id}", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }
    }

}