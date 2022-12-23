namespace Beginor.GisHub.DataServices.Models;

#nullable disable

public class TableModel {
    public string Schema { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

}

public class ColumnModel {
    public string Schema { get; set; }
    public string Table { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int Length { get; set; }
    public bool Nullable { get; set; }
}
