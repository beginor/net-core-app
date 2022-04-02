using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
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

    /// <summary>读取图层的矢量切片信息</summary>
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
            var infoPath = Path.Combine(id.ToString(), "info.json");
            var fileInfo = fileCache.GetFileInfo(infoPath);
            var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{RouteTemplate}/{id}/mvt/{{z}}/{{y}}/{{x}}";
            MvtInfoModel infoModel;
            if (fileInfo.Exists) {
                try {
                    using var reader = fileInfo.OpenText();
                    var text = await reader.ReadToEndAsync();
                    infoModel = JsonSerializer.Deserialize<MvtInfoModel>(text);
                    if (infoModel != null) {
                        infoModel.Url = url;
                        return infoModel;
                    }
                }
                catch (Exception ex) {
                    logger.LogError(ex, $"Can not deserialize {fileInfo.FullName} to {typeof(MvtInfoModel)} !");
                }
            }
            infoModel = new MvtInfoModel {
                LayerName = ds.DataServiceName,
                Description = ds.DataServiceDescription,
                GeometryType = ds.GeometryType,
                Minzoom = ds.MvtMinZoom,
                Maxzoom = ds.MvtMaxZoom
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
                infoModel.Bounds = new[] { ext.Xmin, ext.Ymin, ext.Xmax, ext.Ymax };
            }
            var serializerOptions = new JsonSerializerOptions {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            await fileCache.SetContentAsync(
                infoPath,
                Encoding.UTF8.GetBytes(infoModel.ToJson(serializerOptions))
            );
            infoModel.Url = url;
            return infoModel;
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
            logger.LogError(ex, $"Can not read data as mvt from data service {id}");
            return StatusCode(500);
        }
    }

    /// <summary>判断图层是否支持矢量切片</summary>
    [HttpGet("{id:long}/mvt/support")]
    [Authorize("data_services.read_mvt")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
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
            logger.LogError(ex, $"Can not check mvt support for data service {id}");
            return StatusCode(500);
        }
    }

    /// <summary>获取图层矢量切片缓存的大小</summary>
    [HttpGet("{id:long}/mvt/cache")]
    [Authorize("data_services.read_mvt")]
    [RolesFilter(IdParameterName = "id", ProviderType = typeof(IDataServiceRepository))]
    public async Task<ActionResult<long>> GetCacheSize(long id) {
        try {
            var ds = await repository.GetCacheItemByIdAsync(id);
            if (ds == null) {
                return NotFound();
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                return BadRequest();
            }
            var dirInfo = fileCache.GetDirectoryInfo(id.ToString());
            if (!dirInfo.Exists) {
                return 0L;
            }
            var dirSize = dirInfo.EnumerateFiles("*.mvt", SearchOption.AllDirectories).Aggregate(
                0L,
                (size, fi) => size + fi.Length
            );
            return Ok(dirSize);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get mvt cache size for data service {id}");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除图层的矢量切片缓存</summary>
    [HttpDelete("{id:long}/mvt/cache")]
    [ProducesResponseType(204)]
    [Authorize("data_services.update")]
    public async Task<ActionResult> DeleteMvtCache(long id) {
        var ds = await repository.GetCacheItemByIdAsync(id);
        if (ds == null) {
            return NotFound();
        }
        if (ds.GeometryColumn.IsNullOrEmpty()) {
            return BadRequest();
        }
        try {
            var dirInfo = fileCache.GetDirectoryInfo(ds.DataServiceId.ToString());
            if (dirInfo.Exists) {
                dirInfo.Delete(true);
            }
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete mvt cache for data service {id}");
            return this.InternalServerError(ex);
        }
    }
}
