using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;
using Microsoft.AspNetCore.StaticFiles;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>应用存储 服务接口</summary>
    [ApiController]
    [Route("api/storages")]
    public partial class AppStorageController : Controller {

        private ILogger<AppStorageController> logger;
        private IAppStorageRepository repository;
        private IContentTypeProvider contentTypeProvider;

        public AppStorageController(
            ILogger<AppStorageController> logger,
            IAppStorageRepository repository,
            IContentTypeProvider contentTypeProvider
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                contentTypeProvider = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 应用存储 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("app_storages.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppStorageModel>>> Search(
            [FromQuery]AppStorageSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search app_storages with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary> 创建 应用存储 </summary>
        /// <response code="200">创建 应用存储 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("app_storages.create")]
        public async Task<ActionResult<AppStorageModel>> Create(
            [FromBody]AppStorageModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to app_storages.");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>删除 应用存储 </summary>
        /// <response code="204">删除 应用存储 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_storages.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete app_storages by id {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 获取指定的 应用存储
        /// </summary>
        /// <response code="200">返回 应用存储 信息</response>
        /// <response code="404"> 应用存储 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("app_storages.read_by_id")]
        public async Task<ActionResult<AppStorageModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get app_storages by id {id}.");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 更新 应用存储
        /// </summary>
        /// <response code="200">更新成功，返回 应用存储 信息</response>
        /// <response code="404"> 应用存储 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("app_storages.update")]
        public async Task<ActionResult<AppStorageModel>> Update(
            [FromRoute]long id,
            [FromBody]AppStorageModel model
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
                logger.LogError(ex, $"Can not update app_storages by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }

    }

}
