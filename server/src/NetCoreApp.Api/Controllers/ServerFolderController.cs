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

    /// <summary>服务器目录 服务接口</summary>
    [ApiController]
    [Route("api/server-folders")]
    public partial class ServerFolderController : Controller {

        private ILogger<ServerFolderController> logger;
        private IServerFolderRepository repository;
        private IContentTypeProvider contentTypeProvider;

        public ServerFolderController(
            ILogger<ServerFolderController> logger,
            IServerFolderRepository repository,
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

        /// <summary>搜索 服务器目录 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("server_folders.read")]
        public async Task<ActionResult<PaginatedResponseModel<ServerFolderModel>>> Search(
            [FromQuery]ServerFolderSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search server_folders with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary> 创建 服务器目录 </summary>
        /// <response code="200">创建 服务器目录 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("server_folders.create")]
        public async Task<ActionResult<ServerFolderModel>> Create(
            [FromBody]ServerFolderModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to server_folders.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 服务器目录 </summary>
        /// <response code="204">删除 服务器目录 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("server_folders.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete server_folders by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 服务器目录
        /// </summary>
        /// <response code="200">返回 服务器目录 信息</response>
        /// <response code="404"> 服务器目录 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("server_folders.read_by_id")]
        public async Task<ActionResult<ServerFolderModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get server_folders by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 服务器目录
        /// </summary>
        /// <response code="200">更新成功，返回 服务器目录 信息</response>
        /// <response code="404"> 服务器目录 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("server_folders.update")]
        public async Task<ActionResult<ServerFolderModel>> Update(
            [FromRoute]long id,
            [FromBody]ServerFolderModel model
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
                logger.LogError(ex, $"Can not update server_folders by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
