using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers {

    partial class ServerFolderController {

        [HttpGet("{alias}/browse")]
        [Authorize("server_folders.read_folder_content")]
        public async Task<ActionResult<ServerFolderBrowseModel>> GetFolderContent(
            string alias,
            string path,
            string filter = "*.*"
        ) {
            try {
                var model = await repository.GetFolderContentAsync(
                    new ServerFolderBrowseModel {
                        Alias = alias,
                        Path = path,
                        Filter = filter
                    }
                );
                if (model == null) {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get folder content for {alias}:{path}, {filter} .", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("{alias}/file")]
        [Authorize("server_folders.read_file_content")]
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
