using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.WeChat;

public static class DistributedCacheExtensions {
    public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key) {
        var strJson = await cache.GetStringAsync(key);
        if (string.IsNullOrWhiteSpace(strJson)) {
            return default;
        }
        var res = JsonSerializer.Deserialize<T>(strJson) ?? throw new ApplicationException($"Can not deserialize cache json : {strJson} to {typeof(T)}.");
        return res;
    }
}
