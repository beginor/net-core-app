using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Geo.GeoJson;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IFeatureProvider {

        Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(DataServiceCacheItem dataService, GeoJsonParam param);

        Task<AgsFeatureSet> ReadAsFeatureSetAsync(DataServiceCacheItem dataService, AgsJsonParam param);

        Task<AgsLayerDescription> GetLayerDescriptionAsync(DataServiceCacheItem dataService);

        Task<AgsFeatureSet> QueryAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam);

        Task<int> GetSridAsync(DataServiceCacheItem dataService);

        Task<string> GetGeometryTypeAsync(DataServiceCacheItem dataService);

        Task<bool> SupportVectorTile(DataServiceCacheItem dataService);

        Task<byte[]> ReadAsMvtBuffer(DataServiceCacheItem dataService, int z, int y, int x);

    }

}
