using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.GeoJson;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.Filters;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataSourceController {

        [HttpGet("{id:long}/columns")]
        [Authorize("datasources.read_data")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<List<ColumnModel>>> GetColumns(
            long id
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound($"Datasource {id} does not exits !");
                }
                var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
                var columns = await reader.GetColumnsAsync(dataSource);
                return Json(columns, serializerOptionsFactory.CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get columns of datasource {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/count")]
        [Authorize("datasources.read_data")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<long>> Count(
            [FromRoute]long id,
            [FromQuery]CountParam param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound($"Datasource {id} does not exits !");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
                var count = await reader.CountAsync(dataSource, param);
                return count;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not count datasource {id} with {param.Where} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/data")]
        [Authorize("datasources.read_data")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult> ReadData(
            [FromRoute] long id,
            [FromQuery] ReadDataParam param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = {param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.GroupBy)) {
                    return BadRequest($"$groupBy = {param.GroupBy} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = {param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
                var data = await reader.ReadDataAsync(dataSource, param);
                var total = await reader.CountAsync(dataSource, param);
                var result = new PaginatedResponseModel<IDictionary<string, object>> {
                    Total = total, Data = data, Skip = param.Skip, Take = param.Take
                };
                return Json(result, serializerOptionsFactory.CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data from datasource {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/distinct")]
        [Authorize("datasources.read_data")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult> ReadDistinctData(
            [FromRoute] long id,
            [FromQuery] DistinctParam param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = {param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = {param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
                var data = await reader.ReadDistinctDataAsync(dataSource, param);
                return Json(data, serializerOptionsFactory.CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read distinct data from datasource {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/pivot")]
        [Authorize("datasources.read_data")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult> PivotData(
            long id,
            [FromQuery]PivotParam param
        ) {
            try {
                var dataSource = await repository.GetCacheItemByIdAsync(id);
                if (dataSource == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = {param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Aggregate)) {
                    return BadRequest($"$aggregate = {param.Aggregate} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Field)) {
                    return BadRequest($"$field = {param.Field} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Value)) {
                    return BadRequest($"$pivotValue = {param.Value} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = {param.OrderBy} is not allowed!");
                }
                var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
                var data = await reader.PivotData(dataSource, param);
                return Json(data, serializerOptionsFactory.CreateJsonSerializerOptions());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not pivot data from datasource {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/geojson")]
        [Authorize("datasources.read_geojson")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<GeoJsonFeatureCollection>> ReadAsGeoJson(
            [FromRoute] long id,
            [FromQuery] GeoJsonParam param
        ) {
            try {
                var ds = await repository.GetCacheItemByIdAsync(id);
                if (ds == null) {
                    return NotFound($"Datasource {id} does not exist !");
                }
                if (!ds.HasGeometryColumn) {
                    return BadRequest($"Datasource {id} does not define geometry column !");
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = {param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = {param.OrderBy} is not allowed!");
                }
                //
                var featureProvider = factory.CreateFeatureProvider(ds.DatabaseType);
                var featureCollection = await featureProvider.ReadAsFeatureCollectionAsync(ds, param);
                var json = JsonSerializer.Serialize(featureCollection, typeof(GeoJsonFeatureCollection), serializerOptionsFactory.CreateGeoJsonSerializerOptions());
                return Content(json, "application/geo+json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as geojson from datasource {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id:long}/featureset")]
        [Authorize("datasources.read_featureset")]
        [DataSourceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<AgsFeatureSet>> ReadAsFeatureSet(
            [FromRoute] long id,
            [FromQuery] AgsJsonParam param
        ) {
            try {
                var ds = await repository.GetCacheItemByIdAsync(id);
                if (ds == null) {
                    return NotFound();
                }
                if (!SqlValidator.IsValid(param.Select)) {
                    return BadRequest($"$select = {param.Select} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.Where)) {
                    return BadRequest($"$where = {param.Where} is not allowed!");
                }
                if (!SqlValidator.IsValid(param.OrderBy)) {
                    return BadRequest($"$orderBy = {param.OrderBy} is not allowed!");
                }
                if (!ds.HasGeometryColumn) {
                    return BadRequest($"Datasource {id} does not define geometry column !");
                }
                var reader = factory.CreateFeatureProvider(ds.DatabaseType);
                var fc = await reader.ReadAsFeatureSetAsync(ds, param);
                var json = JsonSerializer.Serialize(fc, typeof(object), serializerOptionsFactory.CreateAgsJsonSerializerOptions());
                return Content(json, "application/json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as FeatureSet from datasource {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
