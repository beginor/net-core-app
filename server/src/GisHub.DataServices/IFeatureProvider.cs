using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Geo.GeoJson;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices; 

public interface IFeatureProvider {

    Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(DataServiceCacheItem dataService, GeoJsonParam param);

    Task<AgsFeatureSet> ReadAsFeatureSetAsync(DataServiceCacheItem dataService, AgsJsonParam param);

    Task<AgsLayerDescription> GetLayerDescriptionAsync(DataServiceCacheItem dataService);

    Task<AgsFeatureSet> QueryAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam);

    Task<int> GetSridAsync(DataServiceCacheItem dataService);

    Task<string> GetGeometryTypeAsync(DataServiceCacheItem dataService);

    Task<bool> SupportMvtAsync(DataServiceCacheItem dataService);

    Task<byte[]> ReadAsMvtBufferAsync(DataServiceCacheItem dataService, int z, int y, int x);
    Task<IList<GeoJsonFeature>> ReadAsGeoJsonAsync(string databaseType, DbDataReader reader, string idField, string geoField);
    IList<GeoJsonFeature> ConvertToGeoJson(IList<IDictionary<string, object>> data, string idField, string geoField);
}