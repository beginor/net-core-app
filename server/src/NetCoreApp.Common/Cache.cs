using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Common {

    public class Cache : Disposable, ICache {

        private IDistributedCache cache;


        public Cache(IDistributedCache cache) {
            this.cache = cache;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                cache = null;
            }
            base.Dispose(disposing);
        }

        public async Task<T> GetItemAsync<T>(string key, CancellationToken token) {
            var buffer = await cache.GetAsync(key, token);
            return JsonSerializer.Deserialize<T>(buffer);
        }

        public Task<string> GetStringAsync(string key, CancellationToken token) {
            return cache.GetStringAsync(key, token);
        }

        public async Task SetItemAsync<T>(string key, T value, CancellationToken token) {
            var str = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, str, token);
        }

        public Task SetStringAsync(string key, string value, CancellationToken token) {
            return cache.SetStringAsync(key, value, token);
        }

        public async Task RemoveAsync(string key, CancellationToken token) {
            await cache.RemoveAsync(key, token);
        }

    }

}
