namespace Beginor.GisHub.DataServices.Data {

    public class DataApiCacheItem {
        public long DataApiId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public bool WriteData { get; set; }
        public string Statement { get; set; }
        public DataApiParameter[] Parameters { get; set; }
        public DataServiceField[] Columns { get; set; }
        public string[] Roles { get; set; }
    }

}