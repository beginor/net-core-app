using System.Data.Common;
using System.Collections.Generic;

namespace Beginor.GisHub.DynamicSql;

public interface IDynamicSqlProvider {

    string BuildDynamicSql(string databaseType, string command, IDictionary<string, object> parameters);

    DbProviderFactory GetDbProviderFactory(string databaseType);

}
