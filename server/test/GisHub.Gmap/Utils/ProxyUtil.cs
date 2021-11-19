using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gmap.Utils {

    public static class ProxyUtil {

        public static async Task<HttpRequestMessage> CreateProxyHttpRequestMessage(HttpRequest request, string proxyGateway) {
            var method = new HttpMethod(request.Method);
            var proxyUrl = proxyGateway;
            if (request.QueryString.HasValue) {
                proxyUrl += request.QueryString.ToString();
            }
            var proxyUri = new Uri(proxyUrl);
            var requestMessage = new HttpRequestMessage(method, proxyUri);
            requestMessage.Headers.UserAgent.ParseAdd(request.Headers.UserAgent);
            requestMessage.Headers.Accept.ParseAdd(request.Headers.Accept);
            requestMessage.Headers.AcceptEncoding.ParseAdd(request.Headers.AcceptEncoding);
            if (requestMessage.Method == HttpMethod.Post) {
                var bodyStream = new MemoryStream();
                await request.BodyReader.CopyToAsync(bodyStream);
                bodyStream.Seek(0, SeekOrigin.Begin);
                var content = new StreamContent(bodyStream);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.Headers.ContentType);
                requestMessage.Content = content;
            }
            return requestMessage;
        }

        public static Dictionary<string, string> ComputeSignatureHeaders(string paasId, string paasToken, string serviceId) {
            var headers = new Dictionary<string, string>();
            var now = DateTimeOffset.Now;
            var timestamp = now.ToUnixTimeSeconds().ToString();
            var random = new Random();
            var nonce = now.ToUnixTimeMilliseconds().ToString("x") + "-" + ((long)Math.Floor(random.NextDouble() * 0xFFFFFF)).ToString("x");
            headers.Add("x-tif-paasId", paasId);
            headers.Add("x-tif-timestamp", timestamp);
            headers.Add("x-tif-nonce", nonce);
            headers.Add("x-tif-signature", ToSha256(timestamp + paasToken + nonce + timestamp));
            if (!string.IsNullOrEmpty(serviceId)) {
                headers.Add("x-tif-serviceId", serviceId);
            }
            return headers;
        }

        public static void AddSignatureHeaders(HttpRequestHeaders headers, string paasId, string paasToken, string serviceId, ILogger logger) {
            var paasHeaders = ProxyUtil.ComputeSignatureHeaders(paasId, paasToken, serviceId);
            logger.LogInformation("Signature headers are:");
            foreach (var pair in paasHeaders) {
                headers.Add(pair.Key, pair.Value);
                logger.LogInformation($"{pair.Key}: {pair.Value}");
            }
        }

        public static bool NeedReplace(string request, string mediaType) {
            return (
                "GetCapabilities".Equals(request, StringComparison.OrdinalIgnoreCase) || "DescribeFeatureType".Equals(request, StringComparison.OrdinalIgnoreCase)
            ) && (
                "text/xml".Equals(mediaType, StringComparison.OrdinalIgnoreCase) || "application/xml".Equals(mediaType, StringComparison.OrdinalIgnoreCase)
            );
        }

        private static Regex regex = new Regex("https://dc-gateway.gdgov.cn(.*?).(?=[\"?])");

        public static async Task ReplaceContent(Stream orginal, Stream result, string replacement) {
            using var reader = new StreamReader(orginal);
            var writer = new StreamWriter(result);
            string line;
            while ((line = await reader.ReadLineAsync()) != null) {
                line = regex.Replace(line, replacement);
                await writer.WriteLineAsync(line);
            }
            await writer.FlushAsync();
        }

        private static string ToSha256(string input) {
            var sha256 = SHA256.Create();
            var buffer = Encoding.UTF8.GetBytes(input);
            var hashed = sha256.ComputeHash(buffer);
            var strBuilder = new StringBuilder();
            for (var i = 0; i < hashed.Length; i++) {
                strBuilder.Append(hashed[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }

}
