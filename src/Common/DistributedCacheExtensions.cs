using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.Common;

public static class DistributedCacheExtensions {

    public static async Task<T?> GetAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken token = default
    ) {
        var buffer = await cache.GetAsync(key, token);
        if (buffer != null) {
            return JsonSerializer.Deserialize<T>(buffer);
        }
        return default;
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        TimeSpan slidingExpiration,
        CancellationToken token = default
    ) {
        var buffer = JsonSerializer.SerializeToUtf8Bytes(
            value,
            new JsonSerializerOptions(JsonSerializerOptions.Default) {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            }
        );
        var options = new DistributedCacheEntryOptions {
            SlidingExpiration = slidingExpiration
        };
        await cache.SetAsync(key, buffer, options, token);
    }
}
