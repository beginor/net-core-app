using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IFeatureProvider {

        Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(DataSourceCacheItem dataSource, GeoJsonParam param);

        Task<AgsFeatureSet> ReadAsFeatureSetAsync(DataSourceCacheItem dataSource, AgsJsonParam param);

        Task<AgsLayerDescription> GetLayerDescriptionAsync(DataSourceCacheItem dataSource);

        Task<AgsFeatureSet> QueryAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam);

        Task<int> GetSridAsync(DataSourceCacheItem dataSource);

        Task<string> GetGeometryTypeAsync(DataSourceCacheItem dataSource);

    }

}
