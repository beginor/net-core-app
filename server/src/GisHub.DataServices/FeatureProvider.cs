using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NetTopologySuite.IO;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.Geo;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Geo.GeoJson;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices;

public abstract class FeatureProvider : IFeatureProvider {

    private static readonly int[] GeographicSrids = { 4326, 4490 };
    private static readonly int[] MercatorSrids = { 3857, 102100 };

    private JsonSerializerOptionsFactory serializerOptionsFactory;

    protected IDataServiceFactory DataServiceFactory { get; }

    protected FeatureProvider(IDataServiceFactory dataServiceFactory, JsonSerializerOptionsFactory serializerOptionsFactory) {
        DataServiceFactory = dataServiceFactory ?? throw new ArgumentNullException(nameof(dataServiceFactory));
        this.serializerOptionsFactory = serializerOptionsFactory ?? throw new ArgumentNullException(nameof(serializerOptionsFactory));
    }

    public async Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(
        DataServiceCacheItem dataService,
        GeoJsonParam param
    ) {
        var dsReader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (dsReader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var rdp = new ReadDataParam {
            Select = CheckGeoSelect(dataService, param.Select),
            Where = CheckGeoWhere(dataService, param.Where),
            OrderBy = param.OrderBy,
            Skip = param.Skip,
            Take = param.Take
        };
        var list = await dsReader.ReadDataAsync(dataService, rdp);
        var result = new GeoJsonFeatureCollection {
            Features = ConvertToGeoJson(list, dataService.PrimaryKeyColumn, dataService.GeometryColumn),
            Crs = new Crs {
                Type = "name",
                Properties = new CrsProperties { Code = dataService.Srid }
            }
        };
        var crsName = "urn:ogc:def:crs:";
        if (IsSupportedGeographicSrid(dataService.Srid)) {
            crsName += "OGC::CRS84";
        }
        else {
            crsName += $"EPSG::{dataService.Srid}";
        }
        result.Crs.Properties.Name = crsName;
        var total = await dsReader.CountAsync(dataService, new CountParam {Where = param.Where});
        result.ExceededTransferLimit = total > list.Count;
        if (param.ReturnBbox) {
            var fs = await QueryForExtentAsync(dataService, new AgsQueryParam { Where = param.Where });
            var ext = fs.Extent;
            result.Bbox = new double[] { ext.Xmin, ext.Ymin, ext.Xmax, ext.Ymax };
        }
        return result;
    }

    public async Task<IList<GeoJsonFeature>> ReadAsGeoJsonAsync(
        string databaseType,
        DbDataReader reader,
        string idField,
        string geoField
    ) {
        var dsReader = DataServiceFactory.CreateDataSourceReader(databaseType);
        if (dsReader == null) {
            throw new NotSupportedException($"Unsupported database type {databaseType}!");
        }
        var data = await dsReader.ReadDataAsync(reader);
        return ConvertToGeoJson(data, idField, geoField);
    }

    public IList<GeoJsonFeature> ConvertToGeoJson(IList<IDictionary<string, object?>> data, string idField, string geoField) {
        var features = new List<GeoJsonFeature>();
        var wktReader = new WKTReader();
        foreach (var dict in data) {
            var id = dict[idField];
            var wkt = (string?) dict[geoField];
            dict.Remove(geoField);
            var feature = new GeoJsonFeature {
                Id = id!,
                Properties = dict!
            };
            if (!string.IsNullOrEmpty(wkt)) {
                var geom = wktReader.Read(wkt);
                feature.Geometry = geom.ToGeoJson();
            }
            features.Add(feature);
        }
        return features;
    }

    public async Task<AgsFeatureSet> ReadAsFeatureSetAsync(DataServiceCacheItem dataService, AgsJsonParam param) {
        var dsReader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (dsReader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var selectFields = CheckGeoSelect(dataService, param.Select);
        var rdp = new ReadDataParam {
            Select = selectFields,
            Where = CheckGeoWhere(dataService, param.Where),
            OrderBy = param.OrderBy,
            Skip = param.Skip,
            Take = param.Take
        };
        var list = await dsReader.ReadDataAsync(dataService, rdp);
        var result = new AgsFeatureSet {
            Features = new List<AgsFeature>(list.Count),
            ObjectIdFieldName = dataService.PrimaryKeyColumn,
            DisplayFieldName = dataService.DisplayColumn,
        };
        if (list.Count <= 0) {
            return result;
        }
        var total = await dsReader.CountAsync(dataService, new CountParam {Where = param.Where});
        if (total > list.Count + param.Skip) {
            result.ExceededTransferLimit = true;
        }
        // var firstRow = list.First();
        var columns = await dsReader.GetColumnsAsync(dataService);
        var fields = selectFields.Split(',');
        columns = columns.Where(c => fields.Contains(c.Name)).ToList();
        result.Fields = new List<AgsField>(columns.Count);
        result.FieldAliases = new Dictionary<string, string>(columns.Count);
        var typeMap = AgsFieldType.FieldTypeMap;
        result.FieldAliases = columns.ToDictionary(
            col => col.Name, col => col.Description ?? col.Name
        );
        result.Fields = await ConvertToFieldsAsync(dataService, columns, dsReader);
        columns.Remove(
            columns.First(col => col.Name.EqualsOrdinalIgnoreCase(dataService.GeometryColumn))
        );
        var reader = new WKTReader();
        foreach (var row in list) {
            var wkt = (string?) row[dataService.GeometryColumn];
            row.Remove(dataService.GeometryColumn);
            var attrs = new Dictionary<string, object>(row.Count);
            foreach (var col in columns) {
                var fieldName = col.Name;
                var fieldVal = row[fieldName];
                if (fieldVal == null) {
                    continue;
                }
                if (fieldName.EqualsOrdinalIgnoreCase(dataService.PrimaryKeyColumn)) {
                    attrs[fieldName] = fieldVal;
                }
                else if (!typeMap.ContainsKey(fieldVal.GetType())) {
                    attrs[fieldName] = JsonSerializer.Serialize(fieldVal);
                }
                else if (fieldVal is DateTime time) {
                    attrs[fieldName] = time.ToUnixTime();
                }
                else if (fieldVal is DateTimeOffset dateTimeOffset) {
                    attrs[fieldName] = dateTimeOffset.ToUnixTimeSeconds();
                }
                else {
                    attrs[fieldName] = fieldVal;
                }
            }
            var feature = new AgsFeature {
                Attributes = attrs
            };
            if (!string.IsNullOrEmpty(wkt)) {
                var geom = reader.Read(wkt);
                feature.Geometry = geom.ToAgs();
            }
            result.Features.Add(feature);
        }
        result.SpatialReference = new AgsSpatialReference {
            Wkid = dataService.Srid
        };
        if (param.ReturnExtent) {
            var fs = await QueryForExtentAsync(dataService, new AgsQueryParam { Where = param.Where });
            result.Extent = fs.Extent;
        }
        TryConvertFeatureSetSr(result, param.OutSR);
        result.GeometryType = GetAgsGeometryType(dataService);
        return result;
    }

    public async Task<AgsLayerDescription> GetLayerDescriptionAsync(DataServiceCacheItem dataService) {
        var reader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (reader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var columns = await reader.GetColumnsAsync(dataService);
        var layerDesc = new AgsLayerDescription {
            CurrentVersion = 10.61,
            Id = 0,
            Name = dataService.DataServiceName,
            Type = "Feature Layer",
            Description = $"Map Service for {dataService.DataServiceName}",
            GeometryType = GetAgsGeometryType(dataService),
            SourceSpatialReference = new AgsSpatialReference { Wkid = dataService.Srid },
            ObjectIdField = dataService.PrimaryKeyColumn,
            DisplayField = dataService.DisplayColumn,
            Fields = await ConvertToFieldsAsync(dataService, columns, reader),
            CanModifyLayer = false,
            CanScaleSymbols = false,
            HasLabels = false,
            Capabilities = "Query,Data",
            MaxRecordCount = 1000,
            SupportsStatistics = true,
            SupportsAdvancedQueries = true,
            SupportedQueryFormatsValue = "JSON,geoJSON",
            IsDataVersioned = false,
            UseStandardizedQueries = true,
            AdvancedQueryCapabilities = new AgsAdvancedQueryCapability {
                UseStandardizedQueries = true,
                SupportsStatistics = true,
                SupportsHavingClause = true,
                SupportsCountDistinct = true,
                SupportsOrderBy = true,
                SupportsDistinct = true,
                SupportsPagination = true,
                SupportsTrueCurve = true,
                SupportsReturningQueryExtent = true,
                SupportsQueryWithDistance = true,
                SupportsSqlExpression = true
            },
        };
        layerDesc.GeometryField = layerDesc.Fields.First(f => f.Type == AgsFieldType.EsriGeometry);
        var provider = DataServiceFactory.CreateFeatureProvider(dataService.DatabaseType);
        var featureset = await QueryForExtentAsync(dataService, new AgsQueryParam());
        layerDesc.Extent = featureset.Extent;
        // layerDesc.DrawingInfo = AgsDrawingInfo.CreateDefaultDrawingInfo(layerDesc.GeometryType);
        // layerDesc.OwnershipBasedAccessControlForFeatures = null;
        return layerDesc;
    }

    public async Task<AgsFeatureSet> QueryAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        AgsFeatureSet result;
        if (queryParam.OutStatistics.IsNotNullOrEmpty()) {
            result = await QueryForStatisticsAsync(dataService, queryParam);
        }
        else if (queryParam.ReturnExtentOnly) {
            result = await QueryForExtentAsync(dataService, queryParam);
        }
        else if (queryParam.ReturnCountOnly) {
            result = await QueryForCountAsync(dataService, queryParam);
        }
        else if (queryParam.ReturnIdsOnly) {
            result = await QueryForIdsAsync(dataService, queryParam);
        }
        else {
            var param = ConvertQueryParams(dataService, queryParam);
            var featureProvider = DataServiceFactory.CreateFeatureProvider(dataService.DatabaseType);
            if (featureProvider == null) {
                throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
            }
            result = await featureProvider.ReadAsFeatureSetAsync(dataService, param);
        }
        return result;
    }

    protected async Task<AgsFeatureSet> QueryForIdsAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        var param = ConvertIdsQueryParam(dataService, queryParam);
        var reader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (reader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var data = await reader.ReadDataAsync(dataService, param);
        var result = new AgsFeatureSet();
        result.ObjectIdFieldName = dataService.PrimaryKeyColumn;
        var objectIds = new List<long>(data.Count);
        foreach (var item in data) {
            var val = item[dataService.PrimaryKeyColumn];
            var objectId = Convert.ToInt64(val);
            objectIds.Add(objectId);
        }
        result.ObjectIds = objectIds.ToArray();
        return result;
    }

    protected async Task<AgsFeatureSet> QueryForCountAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        var result = new AgsFeatureSet();
        var param = ConvertCountQueryParam(dataService, queryParam);
        var reader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (reader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var count = await reader.ReadScalarAsync<long>(dataService, param);
        result.Count = count;
        return result;
    }

    protected async Task<AgsFeatureSet> QueryForExtentAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        var result = new AgsFeatureSet();
        var param = ConvertExtentQueryParam(dataService, queryParam);
        var reader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (reader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var wkt = await reader.ReadScalarAsync<string>(dataService, param);
        if (wkt != null) {
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(wkt);
            var envelop = geometry.EnvelopeInternal;
            var extent = new AgsExtent {
                Xmin = envelop.MinX,
                Ymin = envelop.MinY,
                Xmax = envelop.MaxX,
                Ymax = envelop.MaxY,
                SpatialReference = queryParam.OutSRValue
            };
            result.Extent = extent;
        }
        // if (queryParam.ReturnCountOnly) {
        //     var count = await reader.CountAsync(dataSource, param);
        //     result.Count = count;
        // }
        return result;
    }

    protected abstract ReadDataParam ConvertIdsQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam);

    protected abstract ReadDataParam ConvertExtentQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam);

    protected abstract ReadDataParam ConvertStatisticsQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam);

    protected abstract ReadDataParam ConvertCountQueryParam(DataServiceCacheItem dataService, AgsQueryParam queryParam);
    protected string ConvertQueryGeometryToWkt(AgsQueryParam queryParam, int srid) {
        var geometry = queryParam.GetGeometryValue(serializerOptionsFactory.AgsJsonSerializerOptions);
        if (geometry != null) {
            var convertedGeometry = ConvertGeometrySr(geometry, geometry.SpatialReference.Wkid, srid);
            if (convertedGeometry != null) {
                return convertedGeometry.ToNtsGeometry().AsText();
            }
        }
        return string.Empty;
    }

    protected async Task<AgsFeatureSet> QueryForStatisticsAsync(DataServiceCacheItem dataService, AgsQueryParam queryParam) {
        var param = ConvertStatisticsQueryParam(dataService, queryParam);
        var reader = DataServiceFactory.CreateDataSourceReader(dataService.DatabaseType);
        if (reader == null) {
            throw new NotSupportedException($"Unsupported database type {dataService.DatabaseType}!");
        }
        var data = await reader.ReadDataAsync(dataService, param);
        var result = new AgsFeatureSet {
            Features = new List<AgsFeature>(data.Count)
        };
        var aliases = new Dictionary<string, string>();
        var groupByFields = queryParam.GroupByValue;
        if (groupByFields != null) {
            foreach (var field in groupByFields) {
                aliases[field] = field;
            }
        }
        var stats = queryParam.OutStatisticsValue;
        foreach (var stat in stats) {
            aliases[stat.OutFieldName] = stat.OutFieldName;
        }
        result.FieldAliases = aliases;
        foreach (var item in data) {
            var feature = new AgsFeature { Attributes = item };
            result.Features.Add(feature);
        }
        return result;
    }

    protected string CheckGeoSelect(DataServiceCacheItem dataService, string select) {
        if (select.IsNullOrEmpty()) {
            select = $"{dataService.PrimaryKeyColumn},{dataService.DisplayColumn},{dataService.GeometryColumn}";
        }
        else {
            var cols = select.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!cols.Contains(dataService.GeometryColumn, StringComparer.OrdinalIgnoreCase)) {
                cols.Add(dataService.GeometryColumn);
            }
            if (!cols.Contains(dataService.PrimaryKeyColumn, StringComparer.OrdinalIgnoreCase)) {
                cols.Insert(0, dataService.PrimaryKeyColumn);
            }
            select = string.Join(',', cols);
        }
        return select;
    }

    protected string CheckGeoWhere(DataServiceCacheItem dataService, string where) {
        var geoWhere = $"( {dataService.GeometryColumn} is not null )";
        if (where.IsNotNullOrEmpty()) {
            geoWhere = $"{geoWhere} and ( {where} ) ";
        }
        return geoWhere;
    }

    public abstract Task<int> GetSridAsync(DataServiceCacheItem dataService);

    public abstract Task<string> GetGeometryTypeAsync(DataServiceCacheItem dataService);

    public abstract Task<bool> SupportMvtAsync(DataServiceCacheItem dataService);

    public abstract Task<byte[]> ReadAsMvtBufferAsync(DataServiceCacheItem dataService, int z, int y, int x);

    protected abstract AgsJsonParam ConvertQueryParams(DataServiceCacheItem dataService, AgsQueryParam queryParam);

    private static bool IsSupported(int srid) {
        return IsSupportedGeographicSrid(srid) || IsSupportedMercatorSrid(srid);
    }

    private static bool IsSupportedGeographicSrid(int srid) {
        return GeographicSrids.Contains(srid);
    }

    private static bool IsSupportedMercatorSrid(int srid) {
        return MercatorSrids.Contains(srid);
    }

    private async Task<AgsField[]> ConvertToFieldsAsync(
        DataServiceCacheItem dataService,
        IList<ColumnModel> columns,
        IDataServiceReader reader
    ) {
        var param = new ReadDataParam {
            Select = CheckGeoSelect(dataService, string.Join(',', columns.Select(c => c.Name))),
            Where = CheckGeoWhere(dataService, string.Empty),
            Take = 1
        };
        var data = await reader.ReadDataAsync(dataService, param);
        if (data.Count < 1) {
            throw new InvalidOperationException($"Data service {dataService.DataServiceId} is empty !");
        }
        return ConvertToFields(dataService, columns, data[0]);
    }

    private AgsField[] ConvertToFields(
        DataServiceCacheItem dataService,
        IEnumerable<ColumnModel> columns,
        IDictionary<string, object?> row
    ) {
        return columns.Select(column => ConvertToField(dataService, column, row)).ToArray();
    }

    private AgsField ConvertToField(
        DataServiceCacheItem dataService,
        ColumnModel column,
        IDictionary<string, object?> row
    ) {
        var field = new AgsField {
            Name = column.Name,
            Alias = column.Description ?? column.Name
        };
        if (column.Name.EqualsOrdinalIgnoreCase(dataService.PrimaryKeyColumn)) {
            field.Type = AgsFieldType.EsriOID;
        }
        else if (column.Name.EqualsOrdinalIgnoreCase(dataService.GeometryColumn)) {
            field.Type = AgsFieldType.EsriGeometry;
        }
        else {
            var data = row[column.Name];
            if (data == null) {
                field.Type = AgsFieldType.EsriString;
            }
            else {
                var dataType = data.GetType();
                var fieldTypeMap = AgsFieldType.FieldTypeMap;
                if (fieldTypeMap.ContainsKey(dataType)) {
                    field.Type = fieldTypeMap[dataType]();
                    if (field.Type == AgsFieldType.EsriString) {
                        field.Length = column.Length;
                    }
                }
                else {
                    field.Type = AgsFieldType.EsriString;
                }
            }
        }
        return field;
    }

    private string GetAgsGeometryType(DataServiceCacheItem dataService) {
        var geomType = dataService.GeometryType;
        if (geomType == "point") {
            return AgsGeometryType.Point;
        }
        if (geomType == "multipoint") {
            return AgsGeometryType.MultiPoint;
        }
        if (geomType.EndsWith("linestring")) {
            return AgsGeometryType.Polyline;
        }
        if (geomType.EndsWith("polygon")) {
            return AgsGeometryType.Polygon;
        }
        return geomType;
    }

    private static bool NeedsConvert(int sourceSR, int targetSR) {
        if (!IsSupported(sourceSR)) {
            return false;
        }
        if (!IsSupported(targetSR)) {
            return false;
        }
        if (sourceSR == targetSR) {
            return false;
        }
        if (sourceSR == AgsSpatialReference.CGC2000.Wkid && targetSR == AgsSpatialReference.WGS84.Wkid) {
            return false;
        }
        if (sourceSR == AgsSpatialReference.WGS84.Wkid && targetSR == AgsSpatialReference.CGC2000.Wkid) {
            return false;
        }
        return true;
    }

    private static void TryConvertFeatureSetSr(AgsFeatureSet featureSet, int outSr) {
        if (!IsSupported(outSr)) {
            return;
        }
        var inSr = featureSet.SpatialReference.Wkid;
        if (NeedsConvert(inSr, outSr)) {
            if (GeographicSrids.Contains(inSr) && MercatorSrids.Contains(outSr)) {
                foreach (var feature in featureSet.Features) {
                    feature.Geometry = WebMercator.FromGeographic(feature.Geometry);
                    feature.Geometry.SpatialReference = null;
                }
                if (featureSet.Extent != null) {
                    featureSet.Extent = (AgsExtent)WebMercator.FromGeographic(featureSet.Extent);
                }
            }
            if (MercatorSrids.Contains(inSr) && GeographicSrids.Contains(outSr)) {
                foreach (var feature in featureSet.Features) {
                    feature.Geometry = WebMercator.ToGeographic(feature.Geometry);
                }
                if (featureSet.Extent != null) {
                    featureSet.Extent = (AgsExtent)WebMercator.ToGeographic(featureSet.Extent);
                }
            }
        }
        var sr = new AgsSpatialReference { Wkid = outSr };
        if (featureSet.Extent != null) {
            featureSet.Extent.SpatialReference = sr;
        }
        featureSet.SpatialReference = sr;
    }

    private static AgsGeometry? ConvertGeometrySr(AgsGeometry geometry, int inSr, int outSr) {
        if (!IsSupported(outSr)) {
            return null;
        }
        if (!NeedsConvert(inSr, outSr)) {
            return null;
        }
        if (GeographicSrids.Contains(inSr) && MercatorSrids.Contains(outSr)) {
            return WebMercator.FromGeographic(geometry);
        }
        if (MercatorSrids.Contains(inSr) && GeographicSrids.Contains(outSr)) {
            return WebMercator.ToGeographic(geometry);
        }
        return null;
    }

    protected static bool NeedTransform(int sourceSrId, int targetSrId) {
        return sourceSrId > 0 && targetSrId > 0 && sourceSrId != targetSrId;
    }
}
