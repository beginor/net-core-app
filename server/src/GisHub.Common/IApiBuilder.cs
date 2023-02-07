using System;
using System.Collections.Generic;

namespace Beginor.GisHub.Common;

public interface IApiBuilder<T> {
    public string BuildApiDoc(DocModel<T> model);
}

public class DocModel<T> {
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public IEnumerable<T> Models { get; set; } = Array.Empty<T>();
    public string Token { get; set; } = string.Empty;
    public string? Referer { get; set; }
}
