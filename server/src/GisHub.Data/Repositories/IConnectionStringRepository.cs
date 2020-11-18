using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>数据库连接串 仓储接口</summary>
    public partial interface IConnectionStringRepository : IRepository<ConnectionStringModel, long> {

        /// <summary>搜索 数据库连接串 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<ConnectionStringModel>> SearchAsync(
            ConnectionStringSearchModel model
        );

    }

}