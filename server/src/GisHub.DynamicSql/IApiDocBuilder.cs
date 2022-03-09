using System.Collections.Generic;
using Beginor.GisHub.DynamicSql.Models;

namespace Beginor.GisHub.DynamicSql;

public interface IApiDocBuilder {

    string BuildApiDoc(string pageTitle, string baseUrl, IEnumerable<DataApiModel> models, string token, string referer);

}
