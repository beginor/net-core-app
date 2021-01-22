using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataSourceController {

        [HttpGet("{id:long}/columns")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult<List<ColumnModel>>> GetColumns(
            long id
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var columns = await reader.GetColumnsAsync(id);
                return Json(columns, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get columns of datasource {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/count")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult<long>> Count(
            [FromRoute]long id,
            [FromQuery(Name = "$where")]string where
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var count = await reader.CountAsync(id, where);
                return count;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not count datasource {id} with {where} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/data")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> ReadData(
            [FromRoute] long id,
            [FromQuery(Name = "$select")] string select = "",
            [FromQuery(Name = "$where")] string where = "",
            [FromQuery(Name = "$groupBy")] string groupBy = "",
            [FromQuery(Name = "$orderBy")] string orderBy = "",
            [FromQuery(Name = "$skip")] int skip = 0,
            [FromQuery(Name = "$take")] int take = 10
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.ReadDataAsync(id, select, where, groupBy, orderBy, skip, take);
                var total = await reader.CountAsync(id, where);
                var result = new PaginatedResponseModel<IDictionary<string, object>> {
                    Total = total, Data = data, Skip = skip, Take = take
                };
                return Json(result, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data from datasource {id} with select {select} where {where} groupBy {groupBy} orderBy {orderBy} skip {skip} take {take} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/distinct")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> ReadDistinctData(
            [FromRoute] long id,
            [FromQuery(Name = "$select")] string select = "",
            [FromQuery(Name = "$where")] string where = "",
            [FromQuery(Name = "$orderBy")] string orderBy = ""
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.ReadDistinctDataAsync(id, select, where, orderBy);
                return Json(data, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read distinct data from datasource {id} with {select} {where} {orderBy} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/pivot")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> PivotData(
            long id,
            [FromQuery(Name = "$select")]string select,
            [FromQuery(Name = "$where")]string where,
            [FromQuery(Name = "$aggregate")]string aggregate,
            [FromQuery(Name = "$pivotField")]string pivotField,
            [FromQuery(Name = "$pivotValue")]string pivotValue,
            [FromQuery(Name = "$orderBy")]string orderBy
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.PivotData(id, select, where, aggregate, pivotField, pivotValue, orderBy);
                return Json(data, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read distinct data from datasource {id} with {select} {where} {orderBy} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        private JsonSerializerOptions CreateJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict
            };
            return options;
        }

    }

}
