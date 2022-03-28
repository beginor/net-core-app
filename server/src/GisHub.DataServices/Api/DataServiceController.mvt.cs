using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.DataServices.Api;

partial class DataServiceController {

    /// <summary>读取图层的适量切片信息</summary>
    [HttpGet("{id:long}/mvt/info")]
    [Authorize("data_services.read_mvt")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult<MvtInfoModel>> GetMvtInfo(long id) {
        try {
            var ds = await repository.GetCacheItemByIdAsync(id);
            if (ds == null) {
                return NotFound();
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                return BadRequest($"Data service {id} does not have geometry column!");
            }
            if (!ds.SupportMvt) {
                return BadRequest($"Data service {id} does not support mvt output.");
            }
            var model = new MvtInfoModel {
                LayerName = ds.DataServiceName,
                Description = ds.DataServiceDescription,
                GeometryType = ds.GeometryType,
                Minzoom = ds.MvtMinZoom,
                Maxzoom = ds.MvtMaxZoom,
                Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{RouteTemplate}/{id}/mvt/{{z}}/{{y}}/{{x}}",
            };
            var featureProvider = factory.CreateFeatureProvider(ds.DatabaseType);
            var fs = await featureProvider.QueryAsync(
                ds,
                new AgsQueryParam {
                    ReturnExtentOnly = true,
                    OutSR = 4326
                }
            );
            var ext = fs.Extent;
            if (ext != null) {
                model.Bounds = new[] { ext.Xmin, ext.Ymin, ext.Xmax, ext.Ymax };
            }
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not read mvt info from datasservice {id}");
            return StatusCode(500);
        }
    }

    /// <summary>读取数据服务的空间数据(矢量切片形式)</summary>
    [HttpGet("{id:long}/mvt/{z:int}/{y:int}/{x:int}")]
    [Authorize("data_services.read_mvt")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult> ReadAsMvt(long id, int z, int y, int x) {
        try {
            var ds = await repository.GetCacheItemByIdAsync(id);
            if (ds == null) {
                return NotFound();
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                return BadRequest();
            }
            if (!ds.SupportMvt) {
                return BadRequest($"Data service {id} does not support mvt output.");
            }
            if (z < ds.MvtMinZoom || z > ds.MvtMaxZoom) {
                return NotFound();
            }
            var contentType = "application/vnd.mapbox-vector-tile";
            var cachePath = Path.Combine($"{ds.DataServiceId}", $"{z}", $"{y}", $"{x}.mvt");
            if (ds.MvtCacheDuration > 0) {
                var cachedBuffer = await fileCache.GetContentAsync(cachePath, ds.MvtCacheDuration);
                if (cachedBuffer != null) {
                    Response.Headers.ContentEncoding = "gzip";
                    return File(cachedBuffer, contentType);
                }
            }
            var provider = factory.CreateFeatureProvider(ds.DatabaseType);
            var buffer = await provider.ReadAsMvtBufferAsync(ds, z, y, x);
            if (buffer == null || buffer.Length == 0) {
                return NotFound();
            }
            await fileCache.SetContentAsync(cachePath, buffer);
            Response.Headers.ContentEncoding = "gzip";
            return File(buffer, contentType);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not read data as mvt from datasservice {id}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id:long}/support-mvt")]
    [Authorize("data_services.read_mvt")]
    public async Task<ActionResult<bool>> SupportMvt(long id) {
        try {
            var ds = await repository.GetCacheItemByIdAsync(id);
            if (ds == null) {
                return NotFound();
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                return BadRequest();
            }
            var provider = factory.CreateFeatureProvider(ds.DatabaseType);
            var supportMvt = await provider.SupportMvtAsync(ds);
            return supportMvt;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not check mvt support for datasservice {id}");
            return StatusCode(500);
        }
    }

}
