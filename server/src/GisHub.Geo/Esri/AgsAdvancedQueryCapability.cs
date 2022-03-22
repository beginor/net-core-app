namespace Beginor.GisHub.Geo.Esri; 

public class AgsAdvancedQueryCapability {

    public bool SupportsStatistics { get; set; }

    public bool SupportsOrderBy { get; set; }

    public bool SupportsDistinct { get; set; }

    public bool SupportsPagination { get; set; }

    public bool SupportsTrueCurve { get; set; }

    public bool SupportsReturningQueryExtent { get; set; }

    public bool SupportsQueryWithDistance { get; set; }

    public bool UseStandardizedQueries { get; set; }

    public bool SupportsHavingClause { get; set; }

    public bool SupportsCountDistinct { get; set; }

    public bool SupportsSqlExpression { get; set; }

}