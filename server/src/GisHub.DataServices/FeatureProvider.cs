using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NetTopologySuite.IO;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public abstract class FeatureProvider : IFeatureProvider {

        private static readonly int[] GeographicSrids = { 4326, 4490 };
        private static readonly int[] MercatorSrids = { 3857, 102100 };

        protected IDataServiceFactory Factory { get; }

        protected FeatureProvider(IDataServiceFactory factory) {
            Factory = factory;
        }

        public async Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(
            DataSourceCacheItem dataSource,
            GeoJsonParam param
        ) {
            var dsReader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var rdp = new ReadDataParam {
                Select = CheckGeoSelect(dataSource, param.Select),
                Where = CheckGeoWhere(dataSource, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await dsReader.ReadDataAsync(dataSource, rdp);
            var result = new GeoJsonFeatureCollection {
                Features = new List<GeoJsonFeature>(list.Count)
            };
            foreach (var dict in list) {
                var id = dict[dataSource.PrimaryKeyColumn];
                var wkt = (string) dict[dataSource.GeometryColumn];
                dict.Remove(dataSource.GeometryColumn);
                var feature = new GeoJsonFeature {
                    Id = id,
                    Properties = dict
                };
                var reader = new WKTReader();
                var geom = reader.Read(wkt);
                feature.Geometry = geom.ToGeoJson();
                result.Features.Add(feature);
            }
            result.Crs = new Crs {
                Properties = new CrsProperties {
                    Code = dataSource.Srid
                }
            };
            var total = await dsReader.CountAsync(dataSource, new CountParam {Where = param.Where});
            result.ExceededTransferLimit = total > list.Count;
            if (param.ReturnBbox) {
                var fs = await QueryForExtentAsync(dataSource, new AgsQueryParam { Where = param.Where });
                var ext = fs.Extent;
                result.Bbox = new double[] { ext.Xmin, ext.Ymin, ext.Xmax, ext.Ymax };
            }
            return result;
        }

        public async Task<AgsFeatureSet> ReadAsFeatureSetAsync(DataSourceCacheItem dataSource, AgsJsonParam param) {
            var dsReader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var selectFields = CheckGeoSelect(dataSource, param.Select);
            var rdp = new ReadDataParam {
                Select = selectFields,
                Where = CheckGeoWhere(dataSource, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await dsReader.ReadDataAsync(dataSource, rdp);
            var result = new AgsFeatureSet {
                Features = new List<AgsFeature>(list.Count),
                ObjectIdFieldName = dataSource.PrimaryKeyColumn,
                DisplayFieldName = dataSource.DisplayColumn,
            };
            if (list.Count <= 0) {
                return result;
            }
            var total = await dsReader.CountAsync(dataSource, new CountParam {Where = param.Where});
            if (total > list.Count + param.Skip) {
                result.ExceededTransferLimit = true;
            }
            // var firstRow = list.First();
            var columns = await dsReader.GetColumnsAsync(dataSource);
            var fields = selectFields.Split(',');
            columns = columns.Where(c => fields.Contains(c.Name)).ToList();
            result.Fields = new List<AgsField>(columns.Count);
            result.FieldAliases = new Dictionary<string, string>(columns.Count);
            var typeMap = AgsFieldType.FieldTypeMap;
            result.FieldAliases = columns.ToDictionary(
                col => col.Name, col => col.Description ?? col.Name
            );
            result.Fields = await ConvertToFieldsAsync(dataSource, columns, dsReader);
            columns.Remove(
                columns.First(col => col.Name.EqualsOrdinalIgnoreCase(dataSource.GeometryColumn))
            );
            foreach (var row in list) {
                var wkt = (string) row[dataSource.GeometryColumn];
                row.Remove(dataSource.GeometryColumn);
                var attrs = new Dictionary<string, object>(row.Count);
                foreach (var col in columns) {
                    var fieldName = col.Name;
                    var fieldVal = row[fieldName];
                    if (fieldVal == null) {
                        continue;
                    }
                    if (fieldName.EqualsOrdinalIgnoreCase(dataSource.PrimaryKeyColumn)) {
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
                var reader = new WKTReader();
                var geom = reader.Read(wkt);
                feature.Geometry = geom.ToAgs();
                result.Features.Add(feature);
            }
            result.SpatialReference = new AgsSpatialReference {
                Wkid = dataSource.Srid
            };
            if (param.ReturnExtent) {
                var fs = await QueryForExtentAsync(dataSource, new AgsQueryParam { Where = param.Where });
                result.Extent = fs.Extent;
            }
            TryConvertFeatureSetSr(result, param.OutSR);
            result.GeometryType = GetAgsGeometryType(dataSource);
            return result;
        }

        public async Task<AgsLayerDescription> GetLayerDescriptionAsync(DataSourceCacheItem dataSource) {
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var columns = await reader.GetColumnsAsync(dataSource);
            var layerDesc = new AgsLayerDescription {
                CurrentVersion = 10.61,
                Id = 0,
                Name = dataSource.DataSourceName,
                Type = "Feature Layer",
                Description = $"Map Service for {dataSource.DataSourceName}",
                GeometryType = GetAgsGeometryType(dataSource),
                SourceSpatialReference = new AgsSpatialReference { Wkid = dataSource.Srid },
                ObjectIdField = dataSource.PrimaryKeyColumn,
                DisplayField = dataSource.DisplayColumn,
                Fields = await ConvertToFieldsAsync(dataSource, columns, reader),
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
            var provider = Factory.CreateFeatureProvider(dataSource.DatabaseType);
            var featureset = await QueryForExtentAsync(dataSource, new AgsQueryParam());
            layerDesc.Extent = featureset.Extent;
            // layerDesc.DrawingInfo = AgsDrawingInfo.CreateDefaultDrawingInfo(layerDesc.GeometryType);
            // layerDesc.OwnershipBasedAccessControlForFeatures = null;
            return layerDesc;
        }

        public async Task<AgsFeatureSet> QueryAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            AgsFeatureSet result;
            if (queryParam.OutStatistics.IsNotNullOrEmpty()) {
                result = await QueryForStatisticsAsync(dataSource, queryParam);
            }
            else if (queryParam.ReturnExtentOnly) {
                result = await QueryForExtentAsync(dataSource, queryParam);
            }
            else if (queryParam.ReturnCountOnly) {
                result = await QueryForCountAsync(dataSource, queryParam);
            }
            else if (queryParam.ReturnIdsOnly) {
                result = await QueryForIdsAsync(dataSource, queryParam);
            }
            else {
                var param = ConvertQueryParams(dataSource, queryParam);
                var featureProvider = Factory.CreateFeatureProvider(dataSource.DatabaseType);
                result = await featureProvider.ReadAsFeatureSetAsync(dataSource, param);
            }
            return result;
        }

        protected async Task<AgsFeatureSet> QueryForIdsAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var param = ConvertIdsQueryParam(dataSource, queryParam);
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var data = await reader.ReadDataAsync(dataSource, param);
            var result = new AgsFeatureSet();
            result.ObjectIdFieldName = dataSource.PrimaryKeyColumn;
            var objectIds = new List<long>(data.Count);
            foreach (var item in data) {
                var val = item[dataSource.PrimaryKeyColumn];
                var objectId = Convert.ToInt64(val);
                objectIds.Add(objectId);
            }
            result.ObjectIds = objectIds.ToArray();
            return result;
        }

        protected async Task<AgsFeatureSet> QueryForCountAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new AgsFeatureSet();
            var param = ConvertCountQueryParam(dataSource, queryParam);
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var count = await reader.ReadScalarAsync<long>(dataSource, param);
            result.Count = count;
            return result;
        }

        protected async Task<AgsFeatureSet> QueryForExtentAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new AgsFeatureSet();
            var param = ConvertExtentQueryParam(dataSource, queryParam);
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var wkt = await reader.ReadScalarAsync<string>(dataSource, param);
            if (wkt != null) {
                var wktReader = new WKTReader();
                var geometry = wktReader.Read(wkt.ToString());
                var envelop = geometry.EnvelopeInternal;
                var extent = new AgsExtent {
                    Xmin = envelop.MinX,
                    Ymin = envelop.MinY,
                    Xmax = envelop.MaxX,
                    Ymax = envelop.MaxY,
                    SpatialReference = new AgsSpatialReference {
                        Wkid = dataSource.Srid
                    }
                };
                result.Extent = extent;
            }
            // if (queryParam.ReturnCountOnly) {
            //     var count = await reader.CountAsync(dataSource, param);
            //     result.Count = count;
            // }
            return result;
        }

        protected abstract ReadDataParam ConvertIdsQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam);

        protected abstract ReadDataParam ConvertExtentQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam);

        protected abstract ReadDataParam ConvertStatisticsQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam);

        protected abstract ReadDataParam ConvertCountQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam);
        protected string ConvertQueryGeometryToWkt(AgsQueryParam queryParam, int srid) {
            var geometry = queryParam.GeometryValue;
            if (geometry != null) {
                var convertedGeometry = ConvertGeometrySr(geometry, geometry.SpatialReference.Wkid, srid);
                if (convertedGeometry != null) {
                    return convertedGeometry.ToNtsGeometry().AsText();
                }
            }
            return string.Empty;
        }

        protected async Task<AgsFeatureSet> QueryForStatisticsAsync(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var param = ConvertStatisticsQueryParam(dataSource, queryParam);
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var data = await reader.ReadDataAsync(dataSource, param);
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

        protected string CheckGeoSelect(DataSourceCacheItem dataSource, string select) {
            if (select.IsNullOrEmpty()) {
                select = $"{dataSource.PrimaryKeyColumn},{dataSource.DisplayColumn},{dataSource.GeometryColumn}";
            }
            else {
                var cols = select.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!cols.Contains(dataSource.GeometryColumn, StringComparer.OrdinalIgnoreCase)) {
                    cols.Add(dataSource.GeometryColumn);
                }
                if (!cols.Contains(dataSource.PrimaryKeyColumn, StringComparer.OrdinalIgnoreCase)) {
                    cols.Insert(0, dataSource.PrimaryKeyColumn);
                }
                select = string.Join(',', cols);
            }
            return select;
        }

        protected string CheckGeoWhere(DataSourceCacheItem dataSource, string where) {
            var geoWhere = $"( {dataSource.GeometryColumn} is not null )";
            if (where.IsNotNullOrEmpty()) {
                geoWhere = $"{geoWhere} and ( {where} ) ";
            }
            return geoWhere;
        }

        public abstract Task<int> GetSridAsync(DataSourceCacheItem dataSource);

        public abstract Task<string> GetGeometryTypeAsync(DataSourceCacheItem dataSource);

        protected abstract AgsJsonParam ConvertQueryParams(DataSourceCacheItem dataSource, AgsQueryParam queryParam);

        private static bool IsSupported(int srid) {
            return GeographicSrids.Contains(srid) || MercatorSrids.Contains(srid);
        }

        private async Task<AgsField[]> ConvertToFieldsAsync(
            DataSourceCacheItem dataSource,
            IList<ColumnModel> columns,
            IDataSourceReader reader
        ) {
            var param = new ReadDataParam {
                Select = CheckGeoSelect(dataSource, string.Join(',', columns.Select(c => c.Name))),
                Where = CheckGeoWhere(dataSource, string.Empty),
                Take = 1
            };
            var data = await reader.ReadDataAsync(dataSource, param);
            if (data.Count < 1) {
                throw new InvalidOperationException($"Datasource {dataSource.DataSourceId} is empty !");
            }
            return ConvertToFields(dataSource, columns, data[0]);
        }

        private AgsField[] ConvertToFields(
            DataSourceCacheItem dataSource,
            IEnumerable<ColumnModel> columns,
            IDictionary<string, object> row
        ) {
            return columns.Select(column => ConvertToField(dataSource, column, row)).ToArray();
        }

        private AgsField ConvertToField(
            DataSourceCacheItem dataSource,
            ColumnModel column,
            IDictionary<string, object> row
        ) {
            var field = new AgsField {
                Name = column.Name,
                Alias = column.Description ?? column.Name
            };
            if (column.Name.EqualsOrdinalIgnoreCase(dataSource.PrimaryKeyColumn)) {
                field.Type = AgsFieldType.EsriOID;
            }
            else if (column.Name.EqualsOrdinalIgnoreCase(dataSource.GeometryColumn)) {
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

        private string GetAgsGeometryType(DataSourceCacheItem dataSource) {
            var geomType = dataSource.GeometryType;
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

        private static AgsGeometry ConvertGeometrySr(AgsGeometry geometry, int inSr, int outSr) {
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
    }

}
