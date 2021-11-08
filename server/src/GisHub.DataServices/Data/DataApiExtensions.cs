using System;

namespace Beginor.GisHub.DataServices.Data {

    public static class DataApiExtensions {

        public static DataApiCacheItem ToCacheItem(this DataApi api) {
            return new DataApiCacheItem {
                DataApiId = api.Id,
                Name = api.Name,
                Description = api.Description,
                DatabaseType = api.DataSource.DatabaseType,
                ConnectionString = string.Empty,
                WriteData = api.WriteData,
                Statement = api.Statement,
                Parameters = api.Parameters,
                Columns = api.Columns,
                Roles = api.Roles
            };
        }

        public static DataApi ToApi(DataApiCacheItem cacheItem) {
            throw new NotImplementedException();
        }

    }

}
