using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;
using Beginor.GisHub.DataServices.Models;
using Dapper;
using NetTopologySuite.IO;

namespace Beginor.GisHub.DataServices {

    public abstract class DataSourceReader : Disposable, IDataSourceReader {

        protected IDataServiceFactory Factory { get; private set; }
        protected IDataSourceRepository DataSourceRepo { get; private set; }
        protected IConnectionRepository ConnectionRepo { get; private set; }

        protected DataSourceReader(
            IDataServiceFactory factory,
            IDataSourceRepository dataSourceRepo,
            IConnectionRepository connectionRepo
        ) {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            DataSourceRepo = dataSourceRepo ?? throw new ArgumentNullException(nameof(dataSourceRepo));
            ConnectionRepo = connectionRepo ?? throw new ArgumentNullException(nameof(connectionRepo));
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                Factory = null;
                DataSourceRepo = null;
                ConnectionRepo = null;
            }
            base.Dispose(disposing);
        }

        public async Task<long> CountAsync(long dataSourceId, CountParam param) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            var count = await CountAsync(ds, param);
            return count;
        }

        protected abstract Task<long> CountAsync(DataSourceCacheItem ds, CountParam param);

        public async Task<IList<ColumnModel>> GetColumnsAsync(long dataSourceId) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            var columns = await GetColumnsAsync(ds);
            return columns;
        }

        protected virtual async Task<IList<ColumnModel>> GetColumnsAsync(DataSourceCacheItem ds) {
            var dsModel = await DataSourceRepo.GetByIdAsync(ds.DataSourceId);
            var connModel = await ConnectionRepo.GetByIdAsync(long.Parse(dsModel.Connection.Id));
            var meta = Factory.CreateMetadataProvider(connModel.DatabaseType);
            var columns = await meta.GetColumnsAsync(connModel, ds.Schema, ds.TableName);
            return columns;
        }

        public async Task<IList<IDictionary<string, object>>> PivotData(long dataSourceId, PivotParam param) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            var data = await PivotData(ds, param);
            return data;
        }

        protected abstract Task<IList<IDictionary<string, object>>> PivotData(DataSourceCacheItem ds, PivotParam param);

        public async Task<IList<IDictionary<string, object>>> ReadDataAsync(long dataSourceId, ReadDataParam param) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            var data = await ReadDataAsync(ds, param);
            return data;
        }

        protected abstract Task<IList<IDictionary<string, object>>> ReadDataAsync(DataSourceCacheItem ds, ReadDataParam param);

        public async Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(long dataSourceId, DistinctParam param) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            var data = await ReadDistinctDataAsync(ds, param);
            return data;
        }

        protected abstract Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(DataSourceCacheItem ds, DistinctParam param);

        public virtual async Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(
            long dataSourceId,
            GeoJsonParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            if (!ds.HasGeometryColumn) {
                throw new InvalidOperationException(
                    $"Datasource {dataSourceId} does not has geometry column!"
                );
            }
            var fc = await ReadAsFeatureCollectionAsync(ds, param);
            return fc;
        }

        protected virtual async Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(
            DataSourceCacheItem ds,
            GeoJsonParam param
        ) {
            var rdp = new ReadDataParam {
                Select = CheckGeoSelect(ds, param.Select),
                Where = CheckGeoWhere(ds, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await ReadDataAsync(ds, rdp);
            var result = new GeoJsonFeatureCollection {
                Features = new List<GeoJsonFeature>(list.Count)
            };
            foreach (var dict in list) {
                var id = dict[ds.PrimaryKeyColumn];
                var wkt = (string) dict[ds.GeometryColumn];
                dict.Remove(ds.GeometryColumn);
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
                    Code = await GetSridAsync(ds)
                }
            };
            var total = await CountAsync(ds, new CountParam {Where = param.Where});
            result.ExceededTransferLimit = total > list.Count;
            return result;
        }

        public async Task<AgsFeatureSet> ReadAsFeatureSetAsync(
            long dataSourceId,
            AgsJsonParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            if (!ds.HasGeometryColumn) {
                throw new InvalidOperationException(
                    $"Datasource {dataSourceId} does not has geometry column!"
                );
            }
            return await ReadAsFeatureSetAsync(ds, param);
        }

        protected virtual async Task<AgsFeatureSet> ReadAsFeatureSetAsync(
            DataSourceCacheItem ds,
            AgsJsonParam param
        ) {
            var selectFields = CheckGeoSelect(ds, param.Select);
            var rdp = new ReadDataParam {
                Select = selectFields,
                Where = CheckGeoWhere(ds, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await ReadDataAsync(ds, rdp);
            var result = new AgsFeatureSet {
                Features = new List<AgsFeature>(list.Count),
                ObjectIdFieldName = ds.PrimaryKeyColumn,
                DisplayFieldName = ds.DisplayColumn,
            };
            if (list.Count <= 0) {
                return result;
            }
            var total = await CountAsync(ds, new CountParam {Where = param.Where});
            if (total > list.Count) {
                result.ExceededTransferLimit = true;
            }
            // var firstRow = list.First();
            var columns = await GetColumnsAsync(ds);
            var fields = selectFields.Split(',');
            columns = columns.Where(c => fields.Contains(c.Name)).ToList();
            result.Fields = new List<AgsField>(columns.Count);
            result.FieldAliases = new Dictionary<string, string>(columns.Count);
            var typeMap = AgsFieldType.FieldTypeMap;
            result.FieldAliases = GetColumnAlises(columns);
            result.Fields = await ConvertToFieldsAsync(ds, columns);
            columns.Remove(
                columns.First(col => col.Name.EqualsOrdinalIgnoreCase(ds.GeometryColumn))
            );
            foreach (var row in list) {
                var wkt = (string) row[ds.GeometryColumn];
                row.Remove(ds.GeometryColumn);
                var attrs = new Dictionary<string, object>(row.Count);
                foreach (var col in columns) {
                    var fieldName = col.Name;
                    var fieldVal = row[fieldName];
                    if (fieldName.EqualsOrdinalIgnoreCase(ds.PrimaryKeyColumn)) {
                        attrs[fieldName] = fieldVal;
                    }
                    else if (!typeMap.ContainsKey(fieldVal.GetType())) {
                        attrs[fieldName] = JsonSerializer.Serialize(fieldVal);
                    }
                    else if (fieldVal is DateTime time) {
                        attrs[fieldName] = time.ToUnixTime();
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
                Wkid = await GetSridAsync(ds)
            };
            result.GeometryType = await GetAgsGeometryType(ds);
            return result;
        }

        protected string CheckGeoSelect(DataSourceCacheItem ds, string select) {
            if (select.IsNullOrEmpty()) {
                select = $"{ds.PrimaryKeyColumn},{ds.DisplayColumn},{ds.GeometryColumn}";
            }
            else {
                var cols = select.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!cols.Contains(ds.GeometryColumn, StringComparer.OrdinalIgnoreCase)) {
                    cols.Add(ds.GeometryColumn);
                }
                if (!cols.Contains(ds.PrimaryKeyColumn, StringComparer.OrdinalIgnoreCase)) {
                    cols.Insert(0, ds.PrimaryKeyColumn);
                }
                select = string.Join(',', cols);
            }
            return select;
        }

        protected string CheckGeoWhere(
            DataSourceCacheItem ds,
            string where
        ) {
            var geoWhere = $"( {ds.GeometryColumn} is not null )";
            if (where.IsNotNullOrEmpty()) {
                geoWhere = $"{geoWhere} and ( {where} ) ";
            }
            return geoWhere;
        }

        protected virtual KeyValuePair<string, object> ReadField(
            IDataReader dataReader,
            int fieldIndex
        ) {
            var name = dataReader.GetName(fieldIndex);
            var value = dataReader.GetValue(fieldIndex);
            return new KeyValuePair<string, object>(name, value);
        }
        protected virtual async Task<IList<IDictionary<string, object>>> ReadDataAsync(
            IDbConnection conn,
            string sql
        ) {
            var reader = await conn.ExecuteReaderAsync(sql);
            var result = new List<IDictionary<string, object>>();
            while (reader.Read()) {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++) {
                    var pair = ReadField(reader, i);
                    row.Add(pair.Key, pair.Value);
                }
                result.Add(row);
            }
            return result;
        }

        protected void AppendWhere(
            StringBuilder sql,
            string presetCriteria,
            string where
        ) {
            if (presetCriteria.IsNotNullOrEmpty()) {
                sql.AppendLine($" where ({presetCriteria}) ");
                if (where.IsNotNullOrEmpty()) {
                    sql.Append($" and ({where}) ");
                }
            }
            else if (where.IsNotNullOrEmpty()) {
                sql.AppendLine($" where {where} ");
            }
        }

        protected abstract Task<int> GetSridAsync(DataSourceCacheItem ds);

        protected abstract Task<string> GetGeometryTypeAsync(DataSourceCacheItem ds);

        protected async Task<string> GetAgsGeometryType(DataSourceCacheItem ds) {
            var geomType = await GetGeometryTypeAsync(ds);
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

        public async Task<AgsLayerDescription> GetLayerDescriptionAsync(
            long dataSourceId
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Data source {dataSourceId} does not exit !");
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                throw new InvalidOperationException($"Data source {dataSourceId} does not define a geometry column!");
            }
            return await GetLayerDescriptionAsync(ds);
        }

        protected async Task<AgsLayerDescription> GetLayerDescriptionAsync(
            DataSourceCacheItem ds
        ) {
            var columns = await GetColumnsAsync(ds);
            var layerDesc = new AgsLayerDescription {
                CurrentVersion = 10.61,
                Id = 0,
                Name = ds.DataSourceName,
                Type = "Feature Layer",
                Description = $"Map Service for {ds.DataSourceName}",
                GeometryType = await GetAgsGeometryType(ds),
                SourceSpatialReference = new AgsSpatialReference { Wkid = await GetSridAsync(ds) },
                ObjectIdField = ds.PrimaryKeyColumn,
                DisplayField = ds.DisplayColumn,
                Fields = await ConvertToFieldsAsync(ds, columns),
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
            layerDesc.DrawingInfo = AgsDrawingInfo.CreateDefaultDrawingInfo(layerDesc.GeometryType);
            // layerDesc.OwnershipBasedAccessControlForFeatures = null;
            return layerDesc;
        }

        public Task<AgsFeatureSet> QueryAsync(
            long dataSourceId,
            AgsQueryParam queryParam
        ) {
            throw new NotImplementedException();
        }

        protected IDictionary<string, string> GetColumnAlises(
            IList<ColumnModel> columns
        ) {
            return columns.ToDictionary(
                col => col.Name, col => col.Description ?? col.Name
            );
        }

        protected async Task<AgsField[]> ConvertToFieldsAsync(
            DataSourceCacheItem ds,
            IList<ColumnModel> columns
        ) {
            var param = new ReadDataParam {
                Select = CheckGeoSelect(ds, string.Join(',', columns.Select(c => c.Name))),
                Where = CheckGeoWhere(ds, string.Empty),
                Take = 1
            };
            var data = await ReadDataAsync(ds.DataSourceId, param);
            if (data.Count < 1) {
                throw new InvalidOperationException($"Datasource {ds.DataSourceId} is empty !");
            }
            return ConvertToFields(ds, columns, data[0]);
        }

        private AgsField[] ConvertToFields(
            DataSourceCacheItem ds,
            IList<ColumnModel> columns,
            IDictionary<string, object> row
        ) {
            var fields = new List<AgsField>();
            foreach (var column in columns) {
                var field = ConvertToField(ds, column, row);
                fields.Add(field);
            }
            return fields.ToArray();
        }

        protected AgsField ConvertToField(
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
                var dataType = row[column.Name].GetType();
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
            return field;
        }

        protected abstract AgsJsonParam ConvertQueryParams(DataSourceCacheItem ds, AgsQueryParam queryParam);

    }
}
