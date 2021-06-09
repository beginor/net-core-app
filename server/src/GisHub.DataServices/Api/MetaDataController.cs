using System;
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

    /// <summary>数据库元数据 服务</summary>
    [ApiController]
    [Route("api/metadata")]
    public class MetaDataController : Controller {

        private ILogger<MetaDataController> logger;
        private IDataSourceRepository repository;
        private IDataServiceFactory factory;

        public MetaDataController(
            ILogger<MetaDataController> logger,
            IDataSourceRepository repository,
            IDataServiceFactory factory
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>获取数据库连接状态(能否连接到数据库)</summary>
        /// <response code="200">可以连接到数据库</response>
        /// <response code="404">数据库连接 不存在</response>
        /// <response code="500">无法连接到数据库</response>
        [HttpGet("{id:long}/status")]
        [Authorize("metadata.read_status")]
        public async Task<ActionResult> GetStatus(
            [FromRoute] long id
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateMetadataProvider(model.DatabaseType);
                await provider.GetStatus(model);
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get status by id {id} .");
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
        [Authorize("metadata.read_schemas")]
        public async Task<ActionResult<string[]>> GetSchemas(
            [FromRoute] long id
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateMetadataProvider(model.DatabaseType);
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
        [Authorize("metadata.read_tables")]
        public async Task<ActionResult<TableModel[]>> GetTables(
            [FromRoute] long id,
            [FromQuery] string schema
        ) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var provider = factory.CreateMetadataProvider(model.DatabaseType);
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
        [Authorize("metadata.read_columns")]
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
                var provider = factory.CreateMetadataProvider(model.DatabaseType);
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
