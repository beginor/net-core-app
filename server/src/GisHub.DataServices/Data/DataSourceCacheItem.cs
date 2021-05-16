using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Data {

    public class DataSourceCacheItem {

        public long DataSourceId { get; set; }
        public string DataSourceName { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public string DisplayColumn { get; set; }
        public string GeometryColumn { get; set; }
        public string PresetCriteria { get; set; }
        public string DefaultOrder { get; set; }
        public int Srid { get; set; }
        public string GeometryType { get; set; }
        public string[] Roles { get; set; }
        public DataSourceField[] fields { get; set; }

        public bool HasGeometryColumn => GeometryColumn.IsNotNullOrEmpty();

    }

}
