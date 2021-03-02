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
                Select = CheckGeoSelectFields(ds, param.Select),
                Where = param.Where,
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
            var selectFields = CheckGeoSelectFields(ds, param.Select);
            var rdp = new ReadDataParam {
                Select = selectFields,
                Where = param.Where,
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
            var total = await this.CountAsync(dataSourceId, new CountParam { Where = param.Where });
            if (total > list.Count) {
                result.ExceededTransferLimit = true;
            }
            var firstRow = list.First();
            var columns = await GetColumnsAsync(dataSourceId);
            var fields = selectFields.Split(',');
            columns = columns.Where(c => fields.Contains(c.Name)).ToList();
            result.Fields = new List<AgsField>(columns.Count);
            result.FieldAliases = new Dictionary<string, string>(columns.Count);
            var typeMap = AgsFieldDataTypes.FieldDataTypeMap;
            foreach (var col in columns) {
                result.FieldAliases[col.Name] = col.Description ?? col.Name;
                var field = new AgsField {
                    Name = col.Name,
                    Alias = col.Description ?? col.Name
                };
                if (col.Name.EqualsOrdinalIgnoreCase(ds.PrimaryKeyColumn)) {
                    field.Type = AgsFieldDataTypes.EsriOID;
                }
                else if (col.Name.EqualsOrdinalIgnoreCase(ds.GeometryColumn)) {
                    field.Type = AgsFieldDataTypes.EsriGeometry;
                }
                else {
                    var ft = firstRow[col.Name].GetType();
                    field.Type = typeMap.ContainsKey(ft) ? typeMap[ft]() : AgsFieldDataTypes.EsriString;
                }
                result.Fields.Add(field);
            }
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
            result.SpatialReference = new SpatialReference {
                Wkid = await GetSridAsync(dataSourceId)
            };
            result.GeometryType = await GetAgsGeometryType(dataSourceId);
            return result;
        }

        private async Task<string> GetAgsGeometryType(long dataSourceId) {
            var geomType = await GetGeometryTypeAsync(dataSourceId);
            if (geomType == "point") {
                return "point";
            }
            if (geomType == "multipoint") {
                return "multipoint";
            }
            if (geomType.EndsWith("linestring")) {
                return "polyline";
            }
            if (geomType.EndsWith("polygon")) {
                return "polygon";
            }
            return geomType;
        }

        protected string CheckGeoSelectFields(
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

    }
}
