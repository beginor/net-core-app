using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api {

    /// <summary>数据库连接 服务</summary>
    [ApiController]
    [Route("api/connections")]
    public class ConnectionController : Controller {

        private ILogger<ConnectionController> logger;
        private IConnectionRepository repository;
        private IConnectionFactory factory;

        public ConnectionController(
            ILogger<ConnectionController> logger,
            IConnectionRepository repository,
            IConnectionFactory factory
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                factory = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>获取全部的数据库连接列表</summary>
        [HttpGet("~/api/connections-list")]
        [Authorize("connections.read")]
        public async Task<ActionResult<List<ConnectionModel>>> GetAllForDisplayAsync() {
            try {
                var result = await repository.GetAllForDisplayAsync();
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, "Can not get connection-strings-list .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 数据库连接 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("connections.read")]
        public async Task<ActionResult<PaginatedResponseModel<ConnectionModel>>> Search(
            [FromQuery]ConnectionStringSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search connections with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary> 创建 数据库连接 </summary>
        /// <response code="200">创建 数据库连接 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("connections.create")]
        public async Task<ActionResult<ConnectionModel>> Create(
            [FromBody]ConnectionModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to connections.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 数据库连接 </summary>
        /// <response code="204">删除 数据库连接 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("connections.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete connections by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 数据库连接
        /// </summary>
        /// <response code="200">返回 数据库连接 信息</response>
        /// <response code="404"> 数据库连接 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("connections.read_by_id")]
        public async Task<ActionResult<ConnectionModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get connections by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 数据库连接
        /// </summary>
        /// <response code="200">更新成功，返回 数据库连接 信息</response>
        /// <response code="404"> 数据库连接 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("connections.update")]
        public async Task<ActionResult<ConnectionModel>> Update(
            [FromRoute]long id,
            [FromBody]ConnectionModel model
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
                logger.LogError(ex, $"Can not update connections by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取数据库架构 (schema) 元数据
        /// </summary>
        /// <response code="200">获取成功，返回 获取数据库架构 (schema) 元数据列表</response>
        /// <response code="404">数据库连接 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}/schemas")]
        [Authorize("connections.read_metadata")]
        public async Task<ActionResult<string[]>> GetSchemas(
            [FromRoute] long id
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateProvider(model.DatabaseType);
                var schemas = await provider.GetSchemasAsync(model);
                return schemas.ToArray();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get schemas by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取数据库表/视图元数据
        /// </summary>
        /// <response code="200">获取成功，返回 数据库表/视图元数据列表</response>
        /// <response code="404"> 数据库连接 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}/tables")]
        [Authorize("connections.read_metadata")]
        public async Task<ActionResult<TableModel[]>> GetTables(
            [FromRoute] long id,
            [FromQuery] string schema
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateProvider(model.DatabaseType);
                var tables = await provider.GetTablesAsync(model, schema);
                return tables.ToArray();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tables by id {id} and schema {schema} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取数据库表/视图的列元数据
        /// </summary>
        /// <response code="200">获取成功，返回 数据库表/视图的列元数据列表</response>
        /// <response code="404"> 数据库连接 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}/tables/{tableName}/columns")]
        [Authorize("connections.read_metadata")]
        public async Task<ActionResult<ColumnModel[]>> GetColumns(
            [FromRoute] long id,
            [FromRoute] string tableName,
            [FromQuery] string schema
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateProvider(model.DatabaseType);
                var columns = await provider.GetColumnsAsync(model, schema, tableName);
                return columns.ToArray();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tables by id {id} and schema {schema} and table {tableName}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
