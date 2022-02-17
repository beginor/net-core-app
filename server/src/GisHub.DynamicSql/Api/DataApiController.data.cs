using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Beginor.AppFx.Api;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using Beginor.GisHub.Geo.GeoJson;

namespace Beginor.GisHub.DynamicSql.Api {

    partial class DataApiController {

        /// <summary>调用指定的数据 API 查询数据</summary>
        [HttpGet("{id:long}/data")]
        [Authorize("data_apis.read_data")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        public Task<ActionResult> QueryByGet(long id) {
            return QueryImpl(id);
        }

        /// <summary>调用指定的数据 API 查询数据</summary>
        [HttpPost("{id:long}/data")]
        [Authorize("data_apis.read_data")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<ActionResult> QueryByPost(long id) {
            return QueryImpl(id);
        }

        [HttpGet("{id:long}/geojson")]
        [Authorize("data_apis.read_data")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        public Task<ActionResult<GeoJsonFeatureCollection>> QueryGenJsonByGet(long id) {
            return QueryGeoJsonImpl(id);
        }

        [HttpPost("{id:long}/geojson")]
        [Authorize("data_apis.read_data")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<ActionResult<GeoJsonFeatureCollection>> QueryGeoJsonByPost(long id) {
            return QueryGeoJsonImpl(id);
        }

        /// <summary>读取数据API动态生成的sql语句</summary>
        [HttpGet("{id:long}/sql")]
        [Authorize("data_apis.read_sql")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        public Task<ActionResult> BuildSqlByGet(long id) {
            return BuildSqlImpl(id);
        }

        /// <summary>读取数据API动态生成的sql语句</summary>
        [HttpPost("{id:long}/sql")]
        [Authorize("data_apis.read_sql")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<ActionResult> BuildSqlByPost(long id) {
            return BuildSqlImpl(id);
        }

        /// <summary>获取数据API的输出字段列表</summary>
        [HttpGet("{id:long}/columns")]
        [Authorize("data_apis.read_columns")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        public Task<ActionResult<DataServiceFieldModel>> GetColumnsByGet(long id) {
            return GetColumnsImpl(id);
        }

        /// <summary>获取数据API的输出字段列表</summary>
        [HttpPost("{id:long}/columns")]
        [Authorize("data_apis.read_columns")]
        [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataApiRepository))]
        [Consumes("application/x-www-form-urlencoded")]
        public Task<ActionResult<DataServiceFieldModel>> GetColumnsByPost(long id) {
            return GetColumnsImpl(id);
        }

        private async Task<ActionResult<DataServiceFieldModel>> GetColumnsImpl(long id) {
            try {
                var cacheItem = await repository.GetDataApiCacheItemByIdAsync(id);
                if (cacheItem == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                var parameters = GetParameters(Request, cacheItem.Parameters);
                var columns = await repository.GetColumnsAsync(cacheItem, parameters);
                return Ok(columns);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get columns for api {id}");
                return this.InternalServerError(ex);
            }
        }

        private async Task<ActionResult<GeoJsonFeatureCollection>> QueryGeoJsonImpl(long id) {
            try {
                var api = await repository.GetDataApiCacheItemByIdAsync(id);
                if (api == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                if (api.WriteData) {
                    return BadRequest($"DataApi {id} can not used for query!");
                }
                if (string.IsNullOrEmpty(api.IdColumn) || string.IsNullOrEmpty(api.GeometryColumn)) {
                    return BadRequest($"DataApi {id} doesn't define id column and geometry column, can not query as geojson!");
                }
                var parameters = GetParameters(Request, api.Parameters);
                var features = await repository.QueryGeoJsonAsync(api, parameters);
                var result = new GeoJsonFeatureCollection { Features = features };
                var json = JsonSerializer.Serialize(
                    result,
                    serializerOptionsFactory.GeoJsonSerializerOptions
                );
                return this.CompressedContent(json, "application/geo+json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not invoke api {id} .");
                return this.InternalServerError(ex);
            }
        }
        private async Task<ActionResult> QueryImpl(long id) {
            try {
                var api = await repository.GetDataApiCacheItemByIdAsync(id);
                if (api == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                if (api.WriteData) {
                    return BadRequest($"DataApi {id} can not used for query!");
                }
                var parameters = GetParameters(Request, api.Parameters);
                var data = await repository.QueryAsync(api, parameters);
                var result = new DataApiResultModel { Data = data };
                return this.CompressedJson(result, serializerOptionsFactory.JsonSerializerOptions);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not invoke api {id} .");
                return this.InternalServerError(ex);
            }
        }

        private async Task<ActionResult> BuildSqlImpl(long id) {
            try {
                var cacheItem = await repository.GetDataApiCacheItemByIdAsync(id);
                if (cacheItem == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                var parameters = GetParameters(Request, cacheItem.Parameters);
                var sql = await repository.BuildSqlAsync(cacheItem, parameters);
                return Ok(sql);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not build sql for api {id}");
                return this.InternalServerError(ex);
            }
        }

        private IDictionary<string, object> GetParameters(HttpRequest request, IEnumerable<DataApiParameter> parameters) {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var param in parameters) {
                StringValues values;
                if (!request.Query.TryGetValue(param.Name, out values) && request.HasFormContentType && !request.Form.TryGetValue(param.Name, out values)) {
                    continue;
                }
                if (string.IsNullOrEmpty(values)) {
                    continue;
                }
                var val = ConvertParameter(param.Type, values);
                if (val != null) {
                    result[param.Name] = val;
                }
            }
            return result;
        }

        private object ConvertParameter(string parameterType, string parameterValue) {
            var converter = parameterConverterFactory.GetParameterConverter(parameterType);
            return converter.ConvertParameter(parameterValue);
        }

    }

}
