using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Controllers {

    partial class ServerFolderController {

        [HttpGet("{alias}/browse")]
        public async Task<ActionResult<ServerFolderBrowseModel>> GetFolderContent(
            string alias,
            string path,
            string searchPattern = "*.*"
        ) {
            try {
                var model = await repository.GetFolderContentAsync(alias, path, searchPattern);
                if (model == null) {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get folder content for {alias}:{path}, {searchPattern} .", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{alias}/file")]
        public async Task<ActionResult> GetFileContent(
            string alias,
            string path
        ) {
            if (path.IsNullOrEmpty()) {
                return BadRequest("path is null!");
            }
            try {
                var stream = await repository.GetFileContentAsync(alias, path);
                if (stream == null) {
                    return NotFound();
                }
                if (!contentTypeProvider.TryGetContentType(path, out var contentType)) {
                    contentType = "application/octet-stream";
                };
                return File(stream, contentType, false);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get file content for {alias}:{path} .", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }
    }

}
