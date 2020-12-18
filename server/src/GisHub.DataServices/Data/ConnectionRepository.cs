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
    public partial class ConnectionRepository : HibernateRepository<Connection, ConnectionModel, long>, IConnectionRepository {

        public ConnectionRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 数据库连接串 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ConnectionModel>> SearchAsync(
            ConnectionStringSearchModel model
        ) {
            var query = Session.Query<Connection>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                query = query.Where(e => e.Name.Contains(model.Keywords));
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Select(e => new Connection {
                    Id = e.Id,
                    Name = e.Name,
                    DatabaseType = e.DatabaseType,
                    ServerAddress = e.ServerAddress,
                    ServerPort = e.ServerPort,
                    DatabaseName = e.DatabaseName,
                    Username = e.Username,
                    // Password = e.Password,
                    Timeout = e.Timeout
                })
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<ConnectionModel> {
                Total = total,
                Data = Mapper.Map<IList<ConnectionModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<Connection>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync();
            }
        }

        public async Task<List<ConnectionModel>> GetAllForDisplayAsync() {
            var data = await Session.Query<Connection>()
                .Select(e => new Connection {
                    Id = e.Id,
                    Name = e.Name,
                    DatabaseType = e.DatabaseType
                }).ToListAsync();
            return Mapper.Map<List<ConnectionModel>>(data);
        }

    }

}
