using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Data;

public class DataServiceCacheItem {
    public long DataServiceId { get; set; }
    public string DataServiceName { get; set; }
    public string DataServiceDescription { get; set; }
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
    public DataServiceField[] Fields { get; set; }
    public bool SupportMvt { get; set; }
    public int MvtMinZoom { get; set; }
    public int MvtMaxZoom { get; set; }
    public int MvtCacheDuration { get; set; }

    public bool HasGeometryColumn => GeometryColumn.IsNotNullOrEmpty();

}
