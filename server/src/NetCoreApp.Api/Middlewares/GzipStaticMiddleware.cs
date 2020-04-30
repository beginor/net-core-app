using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class GzipStaticMiddleware {

        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly IContentTypeProvider contentTypeProvider;
        private readonly ILogger<GzipStaticMiddleware> logger;

        public GzipStaticMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            IContentTypeProvider contentTypeProvider,
            ILogger<GzipStaticMiddleware> logger
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.contentTypeProvider = contentTypeProvider ?? throw new ArgumentNullException(nameof(contentTypeProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context) {
            var req = context.Request;
            var reqPath = req.Path.ToString();
            if (string.IsNullOrEmpty(reqPath) || string.IsNullOrEmpty(GetExtension(reqPath))) {
                await next(context);
                return;
            }
            var filePath = Path.Combine(env.WebRootPath, reqPath.Substring(1));
            var zipFilePath = filePath + ".gz";
            if (!File.Exists(zipFilePath)) {
                await next(context);
                return;
            }
            if (!req.GetTypedHeaders().AcceptEncoding.Any(e => e.Value.Equals("gzip", StringComparison.OrdinalIgnoreCase))) {
                await next(context);
                return;
            }
            logger.LogInformation($"Handle request for {filePath} with {zipFilePath}");
            var zipFileInfo = new FileInfo(zipFilePath);
            var fileTime = zipFileInfo.LastWriteTimeUtc.ToFileTime().ToString("X");
            var etag = req.Headers["If-None-Match"].ToString();
            var res = context.Response;
            // check for etag;
            if (fileTime.Equals(etag, StringComparison.Ordinal)) {
                logger.LogInformation("ETag match, return 304.");
                res.StatusCode = StatusCodes.Status304NotModified;
                await res.CompleteAsync();
                return;
            }
            res.StatusCode = StatusCodes.Status200OK;
            res.Headers.ContentLength = zipFileInfo.Length;
            res.Headers["Cache-Control"] = "no-cache";
            res.Headers["ETag"] = fileTime;
            res.ContentType = "application/octet-stream";
            if (contentTypeProvider.TryGetContentType(filePath, out var contentType)) {
                res.ContentType = contentType;
            }
            res.Headers["Content-Encoding"] = "gzip";
            using var zipStream = zipFileInfo.OpenRead();
            var buffer = new byte[1024];
            var readed = 0;
            while ((readed = await zipStream.ReadAsync(buffer, 0, 1024)) > 0) {
                await res.Body.WriteAsync(buffer, 0, readed);
            }
            await res.CompleteAsync();
            logger.LogInformation($"Return 200 with gzip encoding.");
        }

        private static string GetExtension(string path) {
            // Don't use Path.GetExtension as that may throw an exception if there are
            // invalid characters in the path. Invalid characters should be handled
            // by the FileProviders
            if (string.IsNullOrWhiteSpace(path)) {
                return null;
            }
            int index = path.LastIndexOf('.');
            if (index < 0) {
                return null;
            }
            return path.Substring(index);
        }

    }

    public static class GzipStaticExtensions {

        public static IApplicationBuilder UseGzipStatic(this IApplicationBuilder app) {
            app.UseMiddleware<GzipStaticMiddleware>();
            return app;
        }
    }

}
