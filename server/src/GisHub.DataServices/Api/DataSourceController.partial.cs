using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Beginor.GisHub.DataServices.GeoJson;

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
            [FromQuery]CountParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var count = await reader.CountAsync(id, param);
                return count;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not count datasource {id} with {param.Where} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/data")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> ReadData(
            [FromRoute] long id,
            [FromQuery] ReadDataParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = ${param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.GroupBy)) {
                    return BadRequest($"$groupBy = ${param.GroupBy} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = ${param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.ReadDataAsync(id, param);
                var total = await reader.CountAsync(id, param);
                var result = new PaginatedResponseModel<IDictionary<string, object>> {
                    Total = total, Data = data, Skip = param.Skip, Take = param.Take
                };
                return Json(result, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data from datasource {id} with {param.ToJson(CreateJsonSerializerOptions())} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/distinct")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> ReadDistinctData(
            [FromRoute] long id,
            [FromQuery] DistinctParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = ${param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = ${param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.ReadDistinctDataAsync(id, param);
                return Json(data, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read distinct data from datasource {id} with {param.ToJson(CreateJsonSerializerOptions())} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/pivot")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult> PivotData(
            long id,
            [FromQuery]PivotParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = ${param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Aggregate)) {
                    return BadRequest($"$aggregate = ${param.Aggregate} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Field)) {
                    return BadRequest($"$field = ${param.Field} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Value)) {
                    return BadRequest($"$pivotValue = ${param.Value} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = ${param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var data = await reader.PivotData(id, param);
                return Json(data, CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not pivot data from datasource {id} with {param.ToJson(CreateJsonSerializerOptions())} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/geojson")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult<GeoJsonFeatureCollection>> ReadAsGeoJson(
            [FromRoute] long id,
            [FromQuery] GeoJsonParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = ${param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = ${param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var fc = await reader.ReadAsFeatureCollectionAsync(id, param);
                var json = JsonSerializer.Serialize(fc, typeof(object), CreateGeoJsonSerializerOptions());
                return Content(json, "application/geo+json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as geojson from datasource {id} with {param.ToJson(CreateJsonSerializerOptions())} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }
        
        [HttpGet("{id:long}/featureset")]
        // [Authorize("datasources.read_data")]
        public async Task<ActionResult<GeoJsonFeatureCollection>> ReadAsFeatureSet(
            [FromRoute] long id,
            [FromQuery] AgsJsonParam param
        ) {
            try {
                var model = await repository.GetCacheItemByIdAsync(id);
                if (model == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = ${param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = ${param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = ${param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(model.DatabaseType);
                var fc = await reader.ReadAsFeatureSetAsync(id, param);
                var json = JsonSerializer.Serialize(fc, typeof(object), CreateAgsJsonSerializerOptions());
                return Content(json, "application/geo+json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as FeatureSet from datasource {id} with {param.ToJson(CreateJsonSerializerOptions())} .");
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
        
        private JsonSerializerOptions CreateGeoJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict,
                IgnoreNullValues = true
            };
            return options;
        }
        
        private JsonSerializerOptions CreateAgsJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict,
                IgnoreNullValues = true
            };
            return options;
        }

    }

}
