using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.WeChat.Models;

namespace Beginor.NetCoreApp.WeChat.Common;

public class ApiGateway : IDisposable {

    private HttpClient httpClient;
    private bool disposed;
    private readonly ILogger<ApiGateway> logger;

    private readonly IDistributedCache cache;
    public readonly WeChatOption option;

    public ApiGateway(
        IDistributedCache cache,
        ILogger<ApiGateway> logger,
        IOptions<WeChatOption> option
        ) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.option = option.Value ?? throw new ArgumentNullException(nameof(option));
        // create http client;
        httpClient = ProxyUtil.CreateHttpClient(this.option.BaseUrl);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                httpClient.Dispose();
            }
            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            disposed = true;
        }
    }

    ~ApiGateway() {
        Dispose(disposing: false);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<string> GetAccessTokenAsync() {
        var key = $"{option.AppId}_{option.Secret}_{option.GrantType}_access_token";
        var token = await cache.GetStringAsync(key);
        if (token.IsNotNullOrEmpty()) {
            return token!;
        }
        var url = $"/cgi-bin/token?grant_type={option.GrantType}&appid={option.AppId}&secret={option.Secret}";
        var tkRes = await RequestAsync<WeChatAccessTokenResponse>(url);
        var cacheOpts = new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tkRes.ExpiresIn)
        };
        await cache.SetStringAsync(key, tkRes.AccessToken, cacheOpts);
        token = tkRes.AccessToken;
        return token;
    }

    public async Task<T> RequestAsync<T>(string requestUri, string json = "") where T : WeChatBaseResponse {
        var req = new HttpRequestMessage(HttpMethod.Post, requestUri);
        if (json.IsNotNullOrEmpty()) {
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        var res = await SendRequest(req);
        var resJson = await res.Content.ReadAsStringAsync();
        logger.LogInformation($"WeChat response content is: {resJson}");
        var baseRes = JsonSerializer.Deserialize<T>(resJson);
        if (baseRes == null) {
            throw new ApplicationException($"Can not deserialize WeChat response to {typeof(T)}.");
        }
        if (baseRes.ErrCode != 0) {
            throw new ApplicationException($"WeChat Error: {baseRes.ErrCode}, {baseRes.ErrMsg}");
        }
        return baseRes;
    }

    private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request) {
        logger.LogInformation($"Sending WeChat request {request.Method} {request.RequestUri}");
        var response = await httpClient.SendAsync(request);
        logger.LogInformation($"WeChat response status is {response.StatusCode}");
        if (!response.IsSuccessStatusCode) {
            throw new ApplicationException($"WeChat response is {response.StatusCode}, {response.ReasonPhrase}");
        }
        return response;
    }

}
