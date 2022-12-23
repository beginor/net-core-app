using System.Threading.Tasks;

namespace Beginor.GisHub.Gmap.Cache;

public interface ICacheProvider {

    Task SaveTileAsync(string serviceId, int level, int row, int col, byte[] content);

    Task<byte[]> GetTileAsync(string serviceId, int level, int row, int col);

}
