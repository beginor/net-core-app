using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Filters;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataServiceController {

        /// <summary>读取数据服务的空间数据(矢量切片形式)</summary>
        [HttpGet("{id:long}/mvt/{z:int}/{y:int}/{x:int}")]
        [Authorize("data_services.read_mvt")]
        [DataServiceRolesFilter(IdParameterName = "id")]
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
                var provider = factory.CreateFeatureProvider(ds.DatabaseType);
                var buffer = await provider.ReadAsMvtBufferAsync(ds, z, y, x);
                if (buffer == null || buffer.Length == 0) {
                    return NotFound();
                }
                Response.Headers["Content-Encoding"] = "gzip";
                return File(buffer, "application/vnd.mapbox-vector-tile");
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

}
