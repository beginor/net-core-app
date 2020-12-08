using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据库连接串仓储实现</summary>
    public partial class ConnectionStringRepository : HibernateRepository<ConnectionString, ConnectionStringModel, long>, IConnectionStringRepository {

        public ConnectionStringRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 数据库连接串 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ConnectionStringModel>> SearchAsync(
            ConnectionStringSearchModel model
        ) {
            var query = Session.Query<ConnectionString>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                query = query.Where(e => e.Name.Contains(model.Keywords));
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Select(e => new ConnectionString {
                    Id = e.Id,
                    Name = e.Name,
                    DatabaseType = e.DatabaseType,
                    Value = e.Value
                })
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            foreach (var item in data) {
               item.Value = item.Value.Aggregate(new StringBuilder(), (sb, c) => sb.Append("*")).ToString();
            }
            return new PaginatedResponseModel<ConnectionStringModel> {
                Total = total,
                Data = Mapper.Map<IList<ConnectionStringModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<ConnectionString>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync();
            }
        }

        public async Task<List<ConnectionStringModel>> GetAllForDisplayAsync() {
            var data = await Session.Query<ConnectionString>()
                .Select(e => new ConnectionString {
                    Id = e.Id,
                    Name = e.Name,
                    DatabaseType = e.DatabaseType
                }).ToListAsync();
            return Mapper.Map<List<ConnectionStringModel>>(data);
        }

    }

}
