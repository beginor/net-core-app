using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Beginor.GisHub.Gmap.Data;

namespace Beginor.GisHub.Gmap.Services;

public class YztService {

    private readonly ILogger<YztService> logger;
    private EBusOptions options;


    public YztService(
        ILogger<YztService> logger,
        IOptionsMonitor<EBusOptions> monitor
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = monitor.CurrentValue ?? throw new ArgumentNullException(nameof(monitor));
        monitor.OnChange(newVal => options = newVal);
    }

    public string GetGatewayServiceUrl(string resource) {
        if (resource.StartsWith(options.GatewayUrl, StringComparison.OrdinalIgnoreCase)) {
            return resource;
        }
        if (options.Tiles.ContainsKey(resource)) {
            return options.GatewayUrl + options.Tiles[resource];
        }
        var path = resource;
        if (!path.StartsWith('/')) {
            path = '/' + resource;
        }
        return options.GatewayUrl + path;
    }

    public string GetGatewayServiceUrl() {
        return options.GatewayUrl;
    }

    public HttpWebRequest CreateHttpRequest(string url, string method) {
        var request = WebRequest.CreateHttp(url);
        request.Method = method;
        request.ServerCertificateValidationCallback = (s, cert, chain, sslErr) => true;
        request.AutomaticDecompression = DecompressionMethods.All;
        request.AllowAutoRedirect = false;
        var headers = ComputeSignatureHeaders();
        foreach (var pair in headers) {
            request.Headers.Add(pair.Key, pair.Value);
        }
        return request;
    }

    public HttpWebRequest CreateHttpRequest(string url, string method, string serviceId) {
        var request = WebRequest.CreateHttp(url);
        request.Method = method;
        request.ServerCertificateValidationCallback = (s, cert, chain, sslErr) => true;
        request.AutomaticDecompression = DecompressionMethods.All;
        request.AllowAutoRedirect = false;
        var headers = ComputeSignatureHeaders(serviceId);
        foreach (var pair in headers) {
            request.Headers.Add(pair.Key, pair.Value);
        }
        return request;
    }

    public string ReplaceGatewayUrl(string content, string replacement) {
        return content.Replace(options.GatewayUrl, replacement, StringComparison.OrdinalIgnoreCase);
    }

    public Dictionary<string, string> ComputeSignatureHeaders(string serviceId = "") {
        var headers = new Dictionary<string, string>();
        var now = DateTimeOffset.Now;
        var timestamp = now.ToUnixTimeSeconds().ToString();
        var random = new Random();
        var nonce = now.ToUnixTimeMilliseconds().ToString("x") + "-" + ((long)Math.Floor(random.NextDouble() * 0xFFFFFF)).ToString("x");
        headers.Add("x-tif-paasId", options.PaasId);
        headers.Add("x-tif-timestamp", timestamp);
        headers.Add("x-tif-nonce", nonce);
        headers.Add("x-tif-signature", ToSha256(timestamp + options.PaasToken + nonce + timestamp));
        if (!string.IsNullOrEmpty(serviceId) && options.Services.ContainsKey(serviceId)) {
            headers.Add("x-tif-serviceId", serviceId);
        }
        logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(headers));
        return headers;
    }

    private string ToSha256(string input) {
        var sha256 = SHA256.Create();
        var buffer = Encoding.UTF8.GetBytes(input);
        var hashed = sha256.ComputeHash(buffer);
        var strBuilder = new StringBuilder();
        for (var i = 0; i < hashed.Length; i++) {
            strBuilder.Append(hashed[i].ToString("x2"));
        }
        return strBuilder.ToString();
    }

    public async Task GetTileContent(string tileName, Tile tile) {
        var template = GetGatewayServiceUrl(tileName);
        var url = string.Format(template, tile.Z, tile.Y, tile.X);
        var request = CreateHttpRequest(url, "GET");
        try {
            using var response = (HttpWebResponse) await request.GetResponseAsync();
            if (response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) {
                var stream = new MemoryStream();
                var responseStream = response.GetResponseStream();
                if (responseStream != null) {
                    await responseStream.CopyToAsync(stream);
                    await stream.FlushAsync();
                    tile.Content = stream.GetBuffer();
                    tile.ContentType = response.ContentType;
                }
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get content from {tileName}");
            tile.Content = new byte[0];
        }
    }

}
