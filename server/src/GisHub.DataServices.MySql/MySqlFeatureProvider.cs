using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Dapper;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.MySql; 

public class MySqlFeatureProvider : FeatureProvider {

    public MySqlFeatureProvider(
        IDataServiceFactory dataServiceFactory,
        JsonSerializerOptionsFactory serializerOptionsFactory
    ) : base(dataServiceFactory, serializerOptionsFactory) { }

    protected override ReadDataParam ConvertIdsQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        throw new NotImplementedException();
    }

    protected override ReadDataParam ConvertExtentQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        throw new NotImplementedException();
    }

    protected override ReadDataParam ConvertStatisticsQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        throw new NotImplementedException();
    }

    protected override ReadDataParam ConvertCountQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        throw new NotImplementedException();
    }

    public override Task<int> GetSridAsync(DataServiceCacheItem dataService) {
        throw new NotImplementedException();
    }

    public override Task<string> GetGeometryTypeAsync(DataServiceCacheItem dataService) {
        throw new NotImplementedException();
    }

    public override Task<bool> SupportMvtAsync(DataServiceCacheItem dataService) {
        throw new NotImplementedException();
    }

    public override Task<byte[]> ReadAsMvtBufferAsync(DataServiceCacheItem dataService, int z, int y, int x) {
        throw new NotImplementedException();
    }

    protected override AgsJsonParam ConvertQueryParams(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        throw new NotImplementedException();
    }

}