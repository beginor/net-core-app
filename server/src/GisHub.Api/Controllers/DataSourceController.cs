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

    /// <summary>数据源（数据表或视图） 服务接口</summary>
    [ApiController]
    [Route("api/data-sources")]
    public class DataSourceController : Controller {

        private ILogger<DataSourceController> logger;
        private IDataSourceRepository repository;

        public DataSourceController(
            ILogger<DataSourceController> logger,
            IDataSourceRepository repository
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

        /// <summary>搜索 数据源（数据表或视图） ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("data_sources.read")]
        public async Task<ActionResult<PaginatedResponseModel<DataSourceModel>>> Search(
            [FromQuery]DataSourceSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search data_sources with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary> 创建 数据源（数据表或视图） </summary>
        /// <response code="200">创建 数据源（数据表或视图） 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("data_sources.create")]
        public async Task<ActionResult<DataSourceModel>> Create(
            [FromBody]DataSourceModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to data_sources.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 数据源（数据表或视图） </summary>
        /// <response code="204">删除 数据源（数据表或视图） 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("data_sources.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete data_sources by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 数据源（数据表或视图）
        /// </summary>
        /// <response code="200">返回 数据源（数据表或视图） 信息</response>
        /// <response code="404"> 数据源（数据表或视图） 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("data_sources.read")]
        public async Task<ActionResult<DataSourceModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get data_sources by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 数据源（数据表或视图）
        /// </summary>
        /// <response code="200">更新成功，返回 数据源（数据表或视图） 信息</response>
        /// <response code="404"> 数据源（数据表或视图） 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("data_sources.update")]
        public async Task<ActionResult<DataSourceModel>> Update(
            [FromRoute]long id,
            [FromBody]DataSourceModel model
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
                logger.LogError(ex, $"Can not update data_sources by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}