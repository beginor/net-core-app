using Microsoft.AspNetCore.Mvc;

namespace Beginor.GisHub.DataServices.Esri {
    public class AgsCommonParams {
        [FromQuery(Name = "f")]
        public string Format { get; set; } = "json";
        [FromQuery(Name = "callback")]
        public string Callback { get; set; }
    }
}
