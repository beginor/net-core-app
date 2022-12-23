using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Beginor.GisHub.Common;

public static class ControllerExtensions {

    public static TService GetRequiredService<TService>(this ControllerBase controller) where TService : notnull {
        return controller.HttpContext.RequestServices.GetRequiredService<TService>();
    }

    public static ActionResult CompressedContent(
        this ControllerBase controller,
        string content,
        string contentType
    ) {
        return CompressedContent(controller, content, MediaTypeHeaderValue.Parse(contentType));
    }

    public static ActionResult CompressedContent(
        this ControllerBase controller,
        string content,
        string contentType,
        Encoding contentEncoding
    ) {
        var headerValue = MediaTypeHeaderValue.Parse(contentType);
        if (contentEncoding != null) {
            headerValue.Encoding = contentEncoding;
        }
        return CompressedContent(controller, content, headerValue);
    }

    public static ActionResult CompressedContent(
        this ControllerBase controller,
        string content,
        MediaTypeHeaderValue contentType
    ) {
        var options = controller.GetRequiredService<CommonOption>();
        if (!options.Output.Compress) {
            return new ContentResult {
                Content = content,
                ContentType = contentType?.ToString()
            };
        }
        var encoding = contentType.Encoding ?? Encoding.UTF8;
        var buffer = encoding.GetBytes(content);
        var output = new MemoryStream();
        using var zipStream = new GZipStream(output, CompressionMode.Compress);
        var input = new MemoryStream(buffer);
        input.CopyTo(zipStream);
        zipStream.Flush();
        var result = new FileContentResult(output.GetBuffer(), contentType);
        controller.Response.Headers.ContentEncoding = "gzip";
        return result;
    }

    public static ActionResult CompressedJson<TValue>(
        this ControllerBase controller,
        TValue obj,
        JsonSerializerOptions options
    ) {
        var json = JsonSerializer.Serialize(obj, options);
        return CompressedContent(controller, json, "application/json");
    }

}
