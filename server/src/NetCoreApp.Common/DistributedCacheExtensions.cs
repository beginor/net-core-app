using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Beginor.NetCoreApp.Common {

    public static class DistributedCacheExtensions {

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default) {
            var buffer = await cache.GetAsync(key, token);
            if (buffer != null) {
                return JsonSerializer.Deserialize<T>(buffer);
            }
            return default(T);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, CancellationToken token = default) {
            var str = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, str, token);
        }
    }

}
