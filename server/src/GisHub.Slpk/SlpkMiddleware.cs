using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Slpk {

    // middleware for slpk
    public class SlpkMiddleware {

        private readonly RequestDelegate next;
        private readonly SlpkOptions options;
        private ILogger<SlpkMiddleware> logger;

        public SlpkMiddleware(
            RequestDelegate next,
            SlpkOptions options,
            ILogger<SlpkMiddleware> logger
        ) {
            this.next = next;
            this.options = options;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext) {
            if (httpContext.Request.Path.HasValue) {
                try {
                    var handled = await HandleRequestAsync(httpContext);
                    if (!handled) {
                        await next(httpContext);
                    }
                }
                catch (Exception ex) {
                    logger.LogError(
                        ex,
                        $"Handle {httpContext.Request.Path} error."
                    );
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await httpContext.Response.WriteAsync(ex.Message);
                }
            }
            await next(httpContext);
        }

        private async Task<bool> HandleRequestAsync(HttpContext context) {
            var req = context.Request;
            var reqPath = req.Path.Value;
            logger.LogInformation($"Request path {reqPath}");
            var filePath = FindFilePath(reqPath);
            var res = context.Response;
            if (string.IsNullOrEmpty(filePath)) {
                return false;
            }
            logger.LogInformation($"File path is: {filePath}");
            var fileInfo = new FileInfo(filePath);
            var fileTime = fileInfo.LastWriteTimeUtc.ToFileTime().ToString("H");
            var etag = req.Headers["If-None-Match"].ToString();
            if (fileTime.Equals(etag, StringComparison.Ordinal)) {
                res.StatusCode = StatusCodes.Status304NotModified;
                await res.CompleteAsync();
                return true;
            }
            res.StatusCode = StatusCodes.Status200OK;
            res.Headers.ContentLength = fileInfo.Length;
            res.Headers["Cache-Control"] = "no-cache";
            res.Headers["ETag"] = fileTime;
            if (filePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)) {
                if (filePath.EndsWith(".json.gz", StringComparison.OrdinalIgnoreCase)) {
                    res.ContentType = "application/json";
                }
                else {
                    res.ContentType = "application/octet-stream";
                }
                res.Headers["Content-Encoding"] = "gzip";
                var content = new byte[fileInfo.Length];
                using var stream = fileInfo.OpenRead();
                await stream.ReadAsync(content, 0, content.Length);
                await res.Body.WriteAsync(content);
                await res.CompleteAsync();
                return true;
            }
            if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) {
                res.ContentType = "application/json";
                using var stream = fileInfo.OpenText();
                var content = await stream.ReadToEndAsync();
                await res.WriteAsync(content);
                await res.CompleteAsync();
                return true;
            }
            if (filePath.EndsWith(".bin")) {
                res.ContentType = "application/octet-stream";
                var content = new byte[fileInfo.Length];
                using var stream = fileInfo.OpenRead();
                await stream.ReadAsync(content, 0, content.Length);
                await res.Body.WriteAsync(content);
                await res.CompleteAsync();
                return true;
            }
            return false;
        }

        private string FindFilePath(string reqPath) {
            var relPath = reqPath.Substring(1);
            if (string.IsNullOrEmpty(relPath)) {
                return string.Empty;
            }
            if (Path.DirectorySeparatorChar != '/') {
                relPath = relPath.Replace('/', Path.DirectorySeparatorChar);
            }
            var localPath = Path.Combine(options.RootFolder, relPath);
            if (Directory.Exists(localPath)) {
                // try find index file;
                foreach (var indexFile in options.IndexFiles) {
                    var indexFilePath = Path.Combine(localPath, indexFile);
                    if (File.Exists(indexFilePath)) {
                        return indexFilePath;
                    }
                }
            }
            else if (!File.Exists(localPath)) {
                // try find file in extensions.
                foreach (var ext in options.Extensions) {
                    var extFilePath = localPath + ext;
                    if (File.Exists(extFilePath)) {
                        return extFilePath;
                    }
                }
            }
            else if (File.Exists(localPath)) {
                return localPath;
            }
            return string.Empty;
        }

    }

}
