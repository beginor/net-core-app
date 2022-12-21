using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.Common;

public static class DistributedCacheExtensions {

    public static async Task<T?> GetAsync<T>(
        this IDistributedCache cache,
        string key
    ) {
        var buffer = await cache.GetAsync(key);
        if (buffer != null) {
            return JsonSerializer.Deserialize<T>(buffer);
        }
        return default(T);
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        TimeSpan slidingExpiration
    ) {
        var buffer = JsonSerializer.SerializeToUtf8Bytes(value);
        var options = new DistributedCacheEntryOptions {
            SlidingExpiration = slidingExpiration
        };
        await cache.SetAsync(key, buffer, options);
    }
}
