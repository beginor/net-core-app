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

        public abstract Task<long> CountAsync(long dataSourceId, CountParam param);

        public virtual async Task<IList<ColumnModel>> GetColumnsAsync(
            long dataSourceId
        ) {
            var dsModel = await DataSourceRepo.GetByIdAsync(dataSourceId);
            if (dsModel == null) {
                return null;
            }
            var connModel = await ConnectionRepo.GetByIdAsync(
                long.Parse(dsModel.Connection.Id)
            );
            var meta = Factory.CreateMetadataProvider(connModel.DatabaseType);
            var columns = await meta.GetColumnsAsync(connModel, dsModel.Schema, dsModel.TableName);
            return columns;
        }
        public abstract Task<IList<IDictionary<string, object>>> PivotData(
            long dataSourceId,
            PivotParam param
        );
        public abstract Task<IList<IDictionary<string, object>>> ReadDataAsync(
            long dataSourceId,
            ReadDataParam param
        );
        public abstract Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(
            long dataSourceId,
            DistinctParam param
        );

        public virtual async Task<GeoJsonFeatureCollection> ReadAsFeatureCollectionAsync(
            long dataSourceId,
            GeoJsonParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (!ds.HasGeometryColumn) {
                throw new InvalidOperationException(
                    $"Datasource {dataSourceId} does not has geometry column!"
                );
            }
            var rdp = new ReadDataParam {
                Select = CheckGeoSelect(ds, param.Select),
                Where = CheckGeoWhere(ds, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await this.ReadDataAsync(dataSourceId, rdp);
            var result = new GeoJsonFeatureCollection {
                Features = new List<GeoJsonFeature>(list.Count)
            };
            foreach (var dict in list) {
                var id = dict[ds.PrimaryKeyColumn];
                var wkt = (string)dict[ds.GeometryColumn];
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
                    Code = await GetSridAsync(dataSourceId)
                }
            };
            var total = await CountAsync(dataSourceId, new CountParam { Where = param.Where });
            result.ExceededTransferLimit = total > list.Count;
            return result;
        }

        public virtual async Task<AgsFeatureSet> ReadAsFeatureSetAsync(
            long dataSourceId,
            AgsJsonParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (!ds.HasGeometryColumn) {
                throw new InvalidOperationException(
                    $"Datasource {dataSourceId} does not has geometry column!"
                );
            }
            var selectFields = CheckGeoSelect(ds, param.Select);
            var rdp = new ReadDataParam {
                Select = selectFields,
                Where = CheckGeoWhere(ds, param.Where),
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await this.ReadDataAsync(dataSourceId, rdp);
            var result = new AgsFeatureSet {
                Features = new List<AgsFeature>(list.Count),
                ObjectIdFieldName = ds.PrimaryKeyColumn,
                DisplayFieldName = ds.DisplayColumn,
            };
            if (list.Count <= 0) {
                return result;
            }
            var total = await this.CountAsync(
                dataSourceId,
                new CountParam { Where = param.Where }
            );
            if (total > list.Count) {
                result.ExceededTransferLimit = true;
            }
            var firstRow = list.First();
            var columns = await GetColumnsAsync(dataSourceId);
            var fields = selectFields.Split(',');
            columns = columns.Where(c => fields.Contains(c.Name)).ToList();
            result.Fields = new List<AgsField>(columns.Count);
            result.FieldAliases = new Dictionary<string, string>(columns.Count);
            var typeMap = AgsFieldType.FieldTypeMap;
            result.FieldAliases = GetColumnAlises(columns);
            // foreach (var col in columns) {
            //     result.FieldAliases[col.Name] = col.Description ?? col.Name;
            //     var field = new AgsField {
            //         Name = col.Name,
            //         Alias = col.Description ?? col.Name
            //     };
            //     if (col.Name.EqualsOrdinalIgnoreCase(ds.PrimaryKeyColumn)) {
            //         field.Type = AgsFieldType.EsriOID;
            //     }
            //     else if (col.Name.EqualsOrdinalIgnoreCase(ds.GeometryColumn)) {
            //         field.Type = AgsFieldType.EsriGeometry;
            //     }
            //     else {
            //         var ft = firstRow[col.Name].GetType();
            //         field.Type = typeMap.ContainsKey(ft) ? typeMap[ft]() : AgsFieldType.EsriString;
            //     }
            //     result.Fields.Add(field);
            // }
            result.Fields = await ConvertToFieldsAsync(ds, columns);
            columns.Remove(columns.First(col => col.Name.EqualsOrdinalIgnoreCase(ds.GeometryColumn)));
            foreach (var row in list) {
                var wkt = (string)row[ds.GeometryColumn];
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
                Wkid = await GetSridAsync(dataSourceId)
            };
            result.GeometryType = await GetAgsGeometryType(dataSourceId);
            return result;
        }

        protected async Task<string> GetAgsGeometryType(long dataSourceId) {
            var geomType = await GetGeometryTypeAsync(dataSourceId);
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

        protected string CheckGeoSelect(
            DataSourceCacheItem ds,
            string select
        ) {
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

        public abstract Task<int> GetSridAsync(long dataSourceId);

        public abstract Task<string> GetGeometryTypeAsync(long dataSourceId);

        public virtual async Task<AgsLayerDescription> GetLayerDescription(
            long dataSourceId
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (ds.GeometryColumn.IsNullOrEmpty()) {
                throw new InvalidOperationException($"Data source {dataSourceId} does not define a geometry column!");
            }
            var layerDesc = new AgsLayerDescription();
            layerDesc.CurrentVersion = 10.61;
            layerDesc.Id = 0;
            layerDesc.Name = ds.DataSourceName;
            layerDesc.Type = "Feature Layer";
            layerDesc.Description = $"Map Service for {ds.DataSourceName}";
            layerDesc.GeometryType = await GetAgsGeometryType(dataSourceId);
            layerDesc.SourceSpatialReference = new AgsSpatialReference {
                Wkid = await GetSridAsync(dataSourceId)
            };
            layerDesc.ObjectIdField = ds.PrimaryKeyColumn;
            layerDesc.DisplayField = ds.DisplayColumn;
            // columns
            var columns = await GetColumnsAsync(dataSourceId);
            layerDesc.Fields = await ConvertToFieldsAsync(ds, columns);
            layerDesc.GeometryField = layerDesc.Fields.First(f => f.Type == AgsFieldType.EsriGeometry);
            layerDesc.CanModifyLayer = false;
            layerDesc.CanScaleSymbols = false;
            layerDesc.HasLabels = false;
            layerDesc.Capabilities = "Query,Data";
            layerDesc.DrawingInfo = AgsDrawingInfo.CreateDefaultDrawingInfo(layerDesc.GeometryType);
            layerDesc.MaxRecordCount = 1000;
            layerDesc.SupportsStatistics = true;
            layerDesc.SupportsAdvancedQueries = true;
            layerDesc.SupportedQueryFormatsValue = "JSON,geoJSON";
            layerDesc.IsDataVersioned = false;
            // layerDesc.OwnershipBasedAccessControlForFeatures = null;
            layerDesc.UseStandardizedQueries = true;
            layerDesc.AdvancedQueryCapabilities = new AgsAdvancedQueryCapability {
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
            };
            return layerDesc;
        }

        public Task<AgsFeatureSet> Query(
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

        // protected abstract AgsJsonParam ConvertQueryParams(AgsQueryParam queryParam);

    }
}
