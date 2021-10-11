using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.Geo.GeoJson;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.DataServices.Filters;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataServiceController {

        /// <summary>读取数据服务的空间数据(GeoJSON)</summary>
        [HttpGet("{id:long}/geojson")]
        [Authorize("data_services.read_geojson")]
        [DataServiceRolesFilter(IdParameterName = "id")]
        public async Task<ActionResult<GeoJsonFeatureCollection>> ReadAsGeoJson(
            [FromRoute] long id,
            [FromQuery] GeoJsonParam param
        ) {
            try {
                var ds = await repository.GetCacheItemByIdAsync(id);
                if (ds == null) {
                    return NotFound($"data service {id} does not exist !");
                }
                if (!ds.HasGeometryColumn) {
                    return BadRequest($"data service {id} does not define geometry column !");
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
                var json = JsonSerializer.Serialize(featureCollection, typeof(GeoJsonFeatureCollection), serializerOptionsFactory.GeoJsonSerializerOptions);
                return Content(json, "application/geo+json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as geojson from data service {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>读取数据服务的空间数据(EsriJSON)</summary>
        [HttpGet("{id:long}/featureset")]
        [Authorize("data_services.read_featureset")]
        [DataServiceRolesFilter(IdParameterName = "id")]
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
                    return BadRequest($"Data Service {id} does not define geometry column !");
                }
                var featureProvider = factory.CreateFeatureProvider(ds.DatabaseType);
                var featureSet = await featureProvider.ReadAsFeatureSetAsync(ds, param);
                var json = JsonSerializer.Serialize(featureSet, typeof(object), serializerOptionsFactory.AgsJsonSerializerOptions);
                return Content(json, "application/json", Encoding.UTF8);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not read data as FeatureSet from data service {id} with {param.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }
    }

}
