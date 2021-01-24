using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
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

        public virtual async Task<GeoJsonFeatureCollection> ReadAsFeatureCollection(
            long dataSourceId,
            GeoJsonParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (!ds.HasGeometryColumn) {
                throw new InvalidOperationException(
                    $"Datasource {dataSourceId} does not has geometry column!"
                );
            }
            var select = param.Select;
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
            var rdp = new ReadDataParam {
                Select = select,
                Where = param.Where,
                OrderBy = param.OrderBy,
                Skip = param.Skip,
                Take = param.Take
            };
            var list = await this.ReadDataAsync(dataSourceId, rdp);
            var result = new GeoJsonFeatureCollection {
                Crs = new Crs {
                    Properties = new CrsProperties {
                        Code = 0
                    }
                },
                Features = new List<GeoJsonFeature>(list.Count)
            };
            foreach (var dict in list) {
                var id = dict[ds.PrimaryKeyColumn];
                var wkt = (string)dict[ds.GeometryColumn];
                dict.Remove(ds.PrimaryKeyColumn);
                dict.Remove(ds.GeometryColumn);
                var feature = new GeoJsonFeature {
                    Id = id,
                    Properties = dict
                };
                var reader = new WKTReader();
                var geom = reader.Read(wkt);
                feature.Geometry = geom.ToGeoJson();
                dict.Remove(ds.PrimaryKeyColumn);
                dict.Remove(ds.GeometryColumn);
                
                result.Features.Add(feature);
            }
            return result;
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

    }
}
