using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Gmap.Data;

namespace Gmap.Services {

    public class YztService {

        private readonly ILogger<YztService> logger;
        private readonly EBusOptions options;

        public YztService(
            ILogger<YztService> logger,
            EBusOptions options
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
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

        public string ReplaceGatewayUrl(string content, string replacement) {
            return content.Replace(options.GatewayUrl, replacement, StringComparison.OrdinalIgnoreCase);
        }

        public Dictionary<string, string> ComputeSignatureHeaders() {
            var headers = new Dictionary<string, string>();
            var now = DateTimeOffset.Now;
            var timestamp = now.ToUnixTimeSeconds().ToString();
            var random = new Random();
            var nonce = now.ToUnixTimeMilliseconds().ToString("x") + "-" + ((long)Math.Floor(random.NextDouble() * 0xFFFFFF)).ToString("x");
            headers.Add("x-tif-paasId", options.PaasId);
            headers.Add("x-tif-timestamp", timestamp);
            headers.Add("x-tif-nonce", nonce);
            headers.Add("x-tif-signature", ToSha256(timestamp + options.PaasToken + nonce + timestamp));
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
            var request = WebRequest.CreateHttp(url);
            request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
                return true;
            };
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.All;
            var paasHeaders = ComputeSignatureHeaders();
            foreach (var pair in paasHeaders) {
                request.Headers.Add(pair.Key, pair.Value);
            }
            try {
                using var response = (HttpWebResponse) await request.GetResponseAsync();
                if (response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) {
                    var stream = new MemoryStream();
                    await response.GetResponseStream().CopyToAsync(stream);
                    await stream.FlushAsync();
                    tile.Content = stream.GetBuffer();
                    tile.ContentType = response.ContentType;
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get content from {tileName}");
                tile.Content = new byte[0];
            }
        }

    }

}
