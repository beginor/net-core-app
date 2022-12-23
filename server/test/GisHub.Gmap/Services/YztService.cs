using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

    private readonly HttpClient httpClient;

    public YztService(
        ILogger<YztService> logger,
        IOptionsMonitor<EBusOptions> monitor
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        options = monitor.CurrentValue ?? throw new ArgumentNullException(nameof(monitor));
        monitor.OnChange(newVal => options = newVal);
        httpClient = CreateHttpClient();
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

    public async Task<HttpResponseMessage?> SendAsync(HttpRequestMessage request) {
        try {
            var response = await httpClient.SendAsync(request);
            return response;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get response for {request.Method} {request.RequestUri}");
            return null;
        }
    }

    public HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string uri) {
        var req = new HttpRequestMessage(method, uri);
        var headers = ComputeSignatureHeaders();
        foreach (var pair in headers) {
            req.Headers.Add(pair.Key, pair.Value);
        }
        return req;
    }

    public string ReplaceGatewayUrl(string content, string replacement) {
        return content.Replace(options.GatewayUrl, replacement, StringComparison.OrdinalIgnoreCase);
    }

    private Dictionary<string, string> ComputeSignatureHeaders(string serviceId = "") {
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
        foreach (var b in hashed) {
            strBuilder.Append(b.ToString("x2"));
        }
        return strBuilder.ToString();
    }

    public async Task GetTileContent(string tileName, Tile tile) {
        var template = GetGatewayServiceUrl(tileName);
        var url = string.Format(template, tile.Z, tile.Y, tile.X);
        var req = CreateHttpRequestMessage(HttpMethod.Get, url);
        using var res = await SendAsync(req);
        if (res == null) {
            logger.LogWarning($"Can not get response for {tileName}, url is {url}");
            return;
        }
        if (!res.IsSuccessStatusCode) {
            logger.LogWarning($"Server returns status {res.StatusCode} {res.ReasonPhrase} for {tileName} , url is {url}");
            return;
        }
        var mediaType = res.Content.Headers.ContentType?.MediaType ?? "";
        if (mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) {
            tile.Content = await res.Content.ReadAsByteArrayAsync();
            tile.ContentType = mediaType;
        }
        else {
            logger.LogWarning($"Server returns content type is {mediaType}, can not read as image.");
        }
    }

    private static HttpClient CreateHttpClient() {
        var handler = new HttpClientHandler {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = false,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        var client = new HttpClient(handler, true);
        return client;
    }
}
