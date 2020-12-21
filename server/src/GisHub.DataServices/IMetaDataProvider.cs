using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IMetaDataProvider {

        string BuildConnectionString(ConnectionModel model);

        Task GetStatus(ConnectionModel model);

        Task<IList<string>> GetSchemasAsync(ConnectionModel model);

        Task<IList<TableModel>> GetTablesAsync(ConnectionModel model, string schema);

        Task<IList<ColumnModel>> GetColumnsAsync(ConnectionModel model, string schema, string tableName);

    }

}
