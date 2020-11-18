using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>数据源（数据表或视图） 仓储接口</summary>
    public partial interface IDataSourceRepository : IRepository<DataSourceModel, long> {

        /// <summary>搜索 数据源（数据表或视图） ，返回分页结果。</summary>
        Task<PaginatedResponseModel<DataSourceModel>> SearchAsync(
            DataSourceSearchModel model
        );

    }

}