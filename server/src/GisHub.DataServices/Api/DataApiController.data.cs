using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql;
using Microsoft.Extensions.Primitives;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataApiController {

        /// <summary>调用指定的数据API</summary>
        [Route("{id:long}/invoke")]
        [Authorize("data_apis.invoke")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult> InvokeApi(long id) {
            try {
                var cacheItem = await repository.GetCacheItemByIdAsync(id);
                if (cacheItem == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                var parameters = GetParameters(Request, cacheItem.Parameters);
                var result = await repository.InvokeApiAsync(cacheItem, parameters);
                return Json(result, serializerOptionsFactory.JsonSerializerOptions);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not invoke api {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>测试数据API动态生成的sql语句</summary>
        [Route("{id:long}/sql")]
        [Authorize("data_apis.read-sql")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult> BuildSql(long id) {
            try {
                var cacheItem = await repository.GetCacheItemByIdAsync(id);
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

        /// <summary>获取数据API的输出字段列表</summary>
        [Route("{id:long}/columns")]
        [Authorize("data_apis.read-columns")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<DataServiceFieldModel>> GetColumns(long id) {
            try {
                var cacheItem = await repository.GetCacheItemByIdAsync(id);
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
