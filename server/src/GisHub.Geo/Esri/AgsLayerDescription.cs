using System.Collections.Generic;

#nullable disable

namespace Beginor.GisHub.Geo.Esri;

public class AgsLayerDescription {

    public double CurrentVersion { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; } = "Feature Layer";

    public string Description { get; set; }

    public string DefinitionExpression { get; set; }

    public string GeometryType { get; set; }

    public string CopyrightText { get; set; }

    // public RelatedLayer ParentLayer { get; set; }

    // public List<RelatedLayer> SubLayers { get; set; }

    public double? MinScale { get; set; }

    public double? MaxScale { get; set; }

    public bool defaultVisibility { get; set; }

    public AgsExtent Extent { get; set; }

    // public LayerTimeInfo TimeInfo { get; set; }

    public bool HasAttachments { get; set; }

    public string HtmlPopupType { get; set; }

    public string DisplayField { get; set; }

    public bool? CanModifyLayer { get; set; }

    public bool? CanScaleSymbols { get; set; }

    public bool? HasLabels { get; set; }

    public string Capabilities { get; set; } = "Query,Data";

    public int MaxRecordCount { get; set; } = 1000;

    public bool? SupportsStatistics { get; set; }

    public bool? SupportsAdvancedQueries { get; set; }

    public bool? SupportsValidateSQL { get; set; }

    public string SupportedQueryFormatsValue { get; set; } = "JSON,geoJSON";

    public bool? IsDataVersioned { get; set; }

    public IEnumerable<AgsField> Fields { get; set; }

    public AgsField GeometryField { get; set; }

    public AgsAdvancedQueryCapability AdvancedQueryCapabilities { get; set; }

    public bool? UseStandardizedQueries { get; set; }

    public bool? HasZ { get; set; }

    public bool? HasM { get; set; }

    public bool? AllowGeometryUpdates { get; set; }

    public bool? SupportsCalculate { get; set; }

    public bool? SupportsAttachmentsByUploadId { get; set; }

    public bool? SupportsApplyEditsWithGlobalIds { get; set; }

    public bool? SupportsRollbackOnFailures { get; set; }

    public string ObjectIdField { get; set; }

    public string GlobalIdField { get; set; }

    public string TypeIdField { get; set; }

    public AgsSpatialReference SourceSpatialReference { get; set; }

    // public JsonElement DrawingInfo { get; set; }

}
