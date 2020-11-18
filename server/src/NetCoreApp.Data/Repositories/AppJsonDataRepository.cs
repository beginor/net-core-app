using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>json 数据仓储实现</summary>
    public partial class AppJsonDataRepository : HibernateRepository<AppJsonData, AppJsonDataModel, long>, IAppJsonDataRepository {

        public AppJsonDataRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 json 数据 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppJsonDataModel>> SearchAsync(
            AppJsonDataSearchModel model
        ) {
            var query = Session.Query<AppJsonData>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<AppJsonDataModel> {
                Total = total,
                Data = Mapper.Map<IList<AppJsonDataModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task<JsonElement> GetValueByIdAsync(long id) {
            return await Session.Query<AppJsonData>()
                .Where(e => e.Id == id)
                .Select(e => e.Value)
                .FirstOrDefaultAsync();
        }

        public async Task SaveValueAsync(long id, JsonElement value) {
            var entity = new AppJsonData { Id = id, Value = value };
            await Session.SaveOrUpdateAsync(entity);
            Session.Flush();
        }
    }

}
