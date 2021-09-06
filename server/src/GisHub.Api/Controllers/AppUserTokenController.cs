using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers {

    /// <summary>用户凭证 服务接口</summary>
    [ApiController]
    [Route("api/app-user-tokens")]
    public class AppUserTokenController : Controller {

        private ILogger<AppUserTokenController> logger;
        private IAppUserTokenRepository repository;

        public AppUserTokenController(
            ILogger<AppUserTokenController> logger,
            IAppUserTokenRepository repository
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 用户凭证 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("app_user_tokens.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppUserTokenModel>>> Search(
            [FromQuery]AppUserTokenSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search app_user_tokens with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary> 创建 用户凭证 </summary>
        /// <response code="200">创建 用户凭证 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("app_user_tokens.create")]
        public async Task<ActionResult<AppUserTokenModel>> Create(
            [FromBody]AppUserTokenModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to app_user_tokens.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 用户凭证 </summary>
        /// <response code="204">删除 用户凭证 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_user_tokens.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete app_user_tokens by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 用户凭证
        /// </summary>
        /// <response code="200">返回 用户凭证 信息</response>
        /// <response code="404"> 用户凭证 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("app_user_tokens.read_by_id")]
        public async Task<ActionResult<AppUserTokenModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get app_user_tokens by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 用户凭证
        /// </summary>
        /// <response code="200">更新成功，返回 用户凭证 信息</response>
        /// <response code="404"> 用户凭证 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("app_user_tokens.update")]
        public async Task<ActionResult<AppUserTokenModel>> Update(
            [FromRoute]long id,
            [FromBody]AppUserTokenModel model
        ) {
            try {
                var exists = await repository.ExitsAsync(id);
                if (!exists) {
                    return NotFound();
                }
                await repository.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update app_user_tokens by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
