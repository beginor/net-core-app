using System.Threading;
using System.Threading.Tasks;

namespace Beginor.NetCoreApp.Common {

    public interface ICache {

        Task<T> GetItemAsync<T>(string key, CancellationToken token = default);

        Task<string> GetStringAsync(string key, CancellationToken token = default);

        Task SetItemAsync<T>(string key, T value, CancellationToken token = default);

        Task SetStringAsync(string key, string value, CancellationToken token = default);

        Task RemoveAsync(string key, CancellationToken token = default);

    }

}
