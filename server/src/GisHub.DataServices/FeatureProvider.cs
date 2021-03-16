using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;
using Beginor.GisHub.DataServices.Models;
using NetTopologySuite.IO;

namespace Beginor.GisHub.DataServices {

    public abstract class FeatureProvider : IFeatureProvider {

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
                    Code = await GetSridAsync(dataSource)
                }
            };
            var total = await dsReader.CountAsync(dataSource, new CountParam {Where = param.Where});
            result.ExceededTransferLimit = total > list.Count;
            return result;
        }

        public async Task<AgsFeatureSet> ReadAsFeatureSetAsync(
            DataSourceCacheItem dataSource,
            AgsJsonParam param
        ) {
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
            if (total > list.Count) {
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
                Wkid = await GetSridAsync(dataSource)
            };
            result.GeometryType = await GetAgsGeometryType(dataSource);
            return result;
        }

        public async Task<AgsLayerDescription> GetLayerDescriptionAsync(
            DataSourceCacheItem dataSource
        ) {
            var reader = Factory.CreateDataSourceReader(dataSource.DatabaseType);
            var columns = await reader.GetColumnsAsync(dataSource);
            var layerDesc = new AgsLayerDescription {
                CurrentVersion = 10.61,
                Id = 0,
                Name = dataSource.DataSourceName,
                Type = "Feature Layer",
                Description = $"Map Service for {dataSource.DataSourceName}",
                GeometryType = await GetAgsGeometryType(dataSource),
                SourceSpatialReference = new AgsSpatialReference { Wkid = await GetSridAsync(dataSource) },
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
            layerDesc.DrawingInfo = AgsDrawingInfo.CreateDefaultDrawingInfo(layerDesc.GeometryType);
            // layerDesc.OwnershipBasedAccessControlForFeatures = null;
            return layerDesc;
        }

        public Task<AgsFeatureSet> QueryAsync(
            DataSourceCacheItem dataSource,
            AgsQueryParam queryParam
        ) {
            throw new System.NotImplementedException();
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

        protected string CheckGeoWhere(DataSourceCacheItem ds, string where) {
            var geoWhere = $"( {ds.GeometryColumn} is not null )";
            if (where.IsNotNullOrEmpty()) {
                geoWhere = $"{geoWhere} and ( {where} ) ";
            }
            return geoWhere;
        }

        protected abstract Task<int> GetSridAsync(DataSourceCacheItem ds);

        protected abstract Task<string> GetGeometryTypeAsync(DataSourceCacheItem ds);

        protected abstract AgsJsonParam ConvertQueryParams(DataSourceCacheItem ds, AgsQueryParam queryParam);

        private async Task<AgsField[]> ConvertToFieldsAsync(
            DataSourceCacheItem ds,
            IList<ColumnModel> columns,
            IDataSourceReader reader
        ) {
            var param = new ReadDataParam {
                Select = CheckGeoSelect(ds, string.Join(',', columns.Select(c => c.Name))),
                Where = CheckGeoWhere(ds, string.Empty),
                Take = 1
            };
            var data = await reader.ReadDataAsync(ds, param);
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

        private async Task<string> GetAgsGeometryType(DataSourceCacheItem ds) {
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
    }

}
