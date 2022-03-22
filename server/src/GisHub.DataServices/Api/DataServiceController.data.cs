using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Api; 

partial class DataServiceController {

    [HttpGet("{id:long}/columns")]
    [Authorize("data_services.read_data")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult<List<ColumnModel>>> GetColumns(long id) {
        try {
            var dataSource = await repository.GetCacheItemByIdAsync(id);
            if (dataSource == null) {
                return NotFound($"Datasource {id} does not exits !");
            }
            var reader = factory.CreateDataSourceReader(dataSource.DatabaseType);
            var columns = await reader.GetColumnsAsync(dataSource);
            return this.CompressedJson(columns, serializerOptionsFactory.JsonSerializerOptions);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get columns of datasource {id} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    [HttpGet("{id:long}/count")]
    [Authorize("data_services.read_data")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult<long>> Count(
        [FromRoute]long id,
        [ModelBinder(BinderType = typeof(EncryptedModelBinder))]CountParam param
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
    [Authorize("data_services.read_data")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult> ReadData(
        [FromRoute] long id,
        [ModelBinder(BinderType = typeof(EncryptedModelBinder))] ReadDataParam param
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
            return this.CompressedJson(result, serializerOptionsFactory.JsonSerializerOptions);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not read data from datasource {id} with {param.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    [HttpGet("{id:long}/distinct")]
    [Authorize("data_services.read_data")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult> ReadDistinctData(
        [FromRoute] long id,
        [ModelBinder(BinderType = typeof(EncryptedModelBinder))] DistinctParam param
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
            return this.CompressedJson(data, serializerOptionsFactory.JsonSerializerOptions);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not read distinct data from datasource {id} with {param.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    [HttpGet("{id:long}/pivot")]
    [Authorize("data_services.read_data")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult> PivotData(
        long id,
        [ModelBinder(BinderType = typeof(EncryptedModelBinder))]PivotParam param
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
            return this.CompressedJson(data, serializerOptionsFactory.JsonSerializerOptions);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not pivot data from datasource {id} with {param.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

}