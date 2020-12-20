using System.Threading.Tasks;

namespace Beginor.GisHub.DataServices {

    public interface IConnectionProvider {

        Task<string> BuildConnectionStringAsync(long id);

        Task<string[]> GetSchemasAsync(long id);
    }

}
