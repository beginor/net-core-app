using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace Beginor.GisHub.Geo.Esri;

public class AgsCommonParams {
    [FromQuery(Name = "f")]
    public string Format { get; set; } = "json";
    [FromQuery(Name = "callback")]
    public string Callback { get; set; }
}
