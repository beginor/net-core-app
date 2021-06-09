using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IMetaDataProvider {

        string BuildConnectionString(DataSourceModel model);

        Task GetStatus(DataSourceModel model);

        Task<IList<string>> GetSchemasAsync(DataSourceModel model);

        Task<IList<TableModel>> GetTablesAsync(DataSourceModel model, string schema);

        Task<IList<ColumnModel>> GetColumnsAsync(DataSourceModel model, string schema, string tableName);

    }

}
