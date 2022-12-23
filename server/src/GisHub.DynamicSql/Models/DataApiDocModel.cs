#nullable disable

namespace Beginor.GisHub.DynamicSql.Models;

public class DataApiDocModel {

    // [FromQuery]
    public long[] Id { get; set; }

    // [FromQuery]
    public string Title { get; set; }

    // [FromQuery]
    public string Description { get; set; }

    // [FromQuery]
    public string Token { get; set; }

    // [FromQuery]
    public string Referer { get; set; }

    // [FromQuery]
    public string Format { get; set; } = "json";

}
