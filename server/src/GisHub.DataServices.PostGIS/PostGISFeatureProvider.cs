using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Dapper;
using Npgsql;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISFeatureProvider : FeatureProvider {

        private static readonly Dictionary<string, string> StatisticDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            ["count"] = "count",
            ["sum"] = "sum",
            ["min"] = "min",
            ["max"] = "max",
            ["avg"] = "avg",
            ["stddev"] = "stddev",
            ["var"] = "variance"
        };

        public PostGISFeatureProvider(
            IDataServiceFactory dataServiceFactory,
            JsonSerializerOptionsFactory jsonSerializerOptionsFactory
        ) : base(dataServiceFactory, jsonSerializerOptionsFactory) { }

        protected override ReadDataParam ConvertIdsQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new ReadDataParam();
            result.Select = dataSource.PrimaryKeyColumn;
            result.Where = BuildWhere(dataSource, queryParam);
            result.OrderBy = queryParam.OrderByFields;
            result.CheckGeometry = false;
            return result;
        }

        protected override AgsJsonParam ConvertQueryParams(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new AgsJsonParam();
            if (queryParam.OutFields.IsNullOrEmpty() || queryParam.OutFields.Trim() == "*") {
                result.Select = $"{dataSource.PrimaryKeyColumn},{dataSource.DisplayColumn},st_astext({dataSource.GeometryColumn})";
            }
            else {
                result.Select = queryParam.OutFields;
            }
            result.Where = BuildWhere(dataSource, queryParam);
            result.OrderBy = queryParam.OrderByFields;
            result.Skip = queryParam.ResultOffset;
            result.Take = queryParam.ResultRecordCount;
            result.CheckGeometry = false;
            result.OutSR = queryParam.OutSR;
            return result;
        }

        protected override ReadDataParam ConvertExtentQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new ReadDataParam();
            result.Select = $"st_astext(st_extent({dataSource.GeometryColumn})::geometry) as {dataSource.GeometryColumn}";
            result.Where = BuildWhere(dataSource, queryParam);
            result.CheckGeometry = false;
            return result;
        }

        protected override ReadDataParam ConvertStatisticsQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new ReadDataParam();
            if (queryParam.GroupByFieldsForStatistics.IsNotNullOrEmpty()) {
                result.Select = queryParam.GroupByFieldsForStatistics + ",";
                result.GroupBy = queryParam.GroupByFieldsForStatistics;
            }
            var stats = queryParam.OutStatisticsValue
                .Where(stat => StatisticDict.ContainsKey(stat.Type))
                .Select(stat => $"{StatisticDict[stat.Type]}({stat.OnField}) as {stat.OutFieldName}");
            result.Select += string.Join(',', stats);
            result.Where = BuildWhere(dataSource, queryParam);
            if (queryParam.OrderByFields.IsNotNullOrEmpty()) {
                result.OrderBy = queryParam.OrderByFields;
            }
            result.Skip = queryParam.ResultOffset;
            result.Take = queryParam.ResultRecordCount;
            result.CheckGeometry = false;
            return result;
        }

        protected override ReadDataParam ConvertCountQueryParam(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var result = new ReadDataParam();
            result.Select = "count(*) as feature_count";
            result.Where = BuildWhere(dataSource, queryParam);
            result.CheckGeometry = false;
            return result;
        }

        public override async Task<int> GetSridAsync(DataSourceCacheItem dataSource) {
            var sql = new StringBuilder();
            sql.AppendLine($" select st_srid({dataSource.GeometryColumn}) ");
            sql.AppendLine($" from {dataSource.Schema}.{dataSource.TableName} ");
            sql.AppendLine($" where {dataSource.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(dataSource.ConnectionString);
            var srid = await conn.ExecuteScalarAsync<int>(sql.ToString());
            return srid;
        }

        public override async Task<string> GetGeometryTypeAsync(DataSourceCacheItem dataSource) {
            var sql = new StringBuilder();
            sql.AppendLine($" select st_geometrytype({dataSource.GeometryColumn}) ");
            sql.AppendLine($" from {dataSource.Schema}.{dataSource.TableName} ");
            sql.AppendLine($" where {dataSource.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(dataSource.ConnectionString);
            var geoType = await conn.ExecuteScalarAsync<string>(sql.ToString());
            return geoType.Substring(3).ToLowerInvariant();
        }

        private string BuildWhere(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var where = new List<string>();
            // query where
            if (queryParam.Where.IsNotNullOrEmpty() && queryParam.Where.Trim() != "1=1") {
                where.Add($"{queryParam.Where}");
            }
            // query objectids
            if (queryParam.ObjectIds.IsNotNullOrEmpty()) {
                where.Add($"{dataSource.PrimaryKeyColumn} in ({queryParam.ObjectIds})");
            }
            // geom query;
            var geomCriteria = AppendGeometryCriteria(dataSource, queryParam);
            if (geomCriteria.IsNotNullOrEmpty()) {
                where.Add($"{geomCriteria}");
            }
            return string.Join(" and ", where);
        }

        private string AppendGeometryCriteria(DataSourceCacheItem dataSource, AgsQueryParam queryParam) {
            var srid = dataSource.Srid;
            var wkt = ConvertQueryGeometryToWkt(queryParam, srid);
            if (wkt.IsNullOrEmpty()) {
                return string.Empty;
            }
            var relFuncName = GetSpatialRelFuncName(queryParam.SpatialRel);
            var geomCol = dataSource.GeometryColumn;
            return $" {relFuncName}({geomCol}, ST_GeomFromText('{wkt}', {srid})::geometry) = true ";
        }

        private static string GetSpatialRelFuncName(string spatialRel) {
            string functionName;
            switch (spatialRel) {
                case AgsSpatialRelationshipType.Contains:
                    functionName = "ST_Contains";
                    break;
                case AgsSpatialRelationshipType.Crosses:
                    functionName = "ST_Crosses";
                    break;
                //                case AgsSpatialRelationshipType.EnvelopeIntersects:
                //                    functionName = "";
                //                    break;
                //                case AgsSpatialRelationshipType.IndexIntersects:
                //                    functionName = "";
                //                    break;
                case AgsSpatialRelationshipType.Overlaps:
                    functionName = "ST_Overlaps";
                    break;
                case AgsSpatialRelationshipType.Touches:
                    functionName = "ST_Touches";
                    break;
                case AgsSpatialRelationshipType.Within:
                    functionName = "ST_Within";
                    break;
                case AgsSpatialRelationshipType.Relation:
                    functionName = "ST_Relate";
                    break;
                default:
                    functionName = "ST_Intersects";
                    break;
            }
            return functionName;
        }
    }

}
