using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>服务器目录仓储实现</summary>
    public partial class ServerFolderRepository : HibernateRepository<ServerFolder, ServerFolderModel, long>, IServerFolderRepository {

        public ServerFolderRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 服务器目录 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ServerFolderModel>> SearchAsync(
            ServerFolderSearchModel model
        ) {
            var query = Session.Query<ServerFolder>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<ServerFolderModel> {
                Total = total,
                Data = Mapper.Map<IList<ServerFolderModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}