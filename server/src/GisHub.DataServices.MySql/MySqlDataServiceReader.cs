using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using MySql.Data.MySqlClient;

namespace Beginor.GisHub.DataServices.MySql {

    public class MySqlDataServiceReader : DataServiceReader {

        public MySqlDataServiceReader(
            IDataServiceFactory factory,
            IDataServiceRepository dataServiceRepo,
            IDataSourceRepository dataSourceRepo,
            ILogger<DataServiceReader> logger
        ) : base(factory, dataServiceRepo, dataSourceRepo, logger) { }

        public override IDbConnection CreateConnection(DataServiceCacheItem dataService) {
            return new MySqlConnection(dataService.ConnectionString);
        }

        protected override string BuildReadDataSql(DataServiceCacheItem dataService, ReadDataParam param) {
            throw new NotImplementedException();
        }

        protected override string BuildCountSql(DataServiceCacheItem dataService, CountParam param) {
            throw new NotImplementedException();
        }

        protected override string BuildDistinctSql(DataServiceCacheItem dataService, DistinctParam param) {
            throw new NotImplementedException();
        }

        protected override string BuildPivotSql(DataServiceCacheItem dataService, PivotParam param) {
            throw new NotImplementedException();
        }

        protected override string BuildScalarSql(DataServiceCacheItem dataService, ReadDataParam param) {
            throw new NotImplementedException();
        }

    }

}
