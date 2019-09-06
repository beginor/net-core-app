using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>审计日志服务实现</summary>
    public partial class AppAuditLogService : BaseService<IAppAuditLogRepository, AppAuditLog, AppAuditLogModel, long>, IAppAuditLogService {

        public AppAuditLogService(IAppAuditLogRepository repository, IMapper mapper) : base(repository, mapper) { }

        protected override long ConvertIdFromString(string id) {
            long result;
            if (long.TryParse(id, out result)) {
                return result;
            }
            return result;
        }

        /// <summary>审计日志搜索，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
            AppAuditLogSearchModel model
        ) {
            var repo = base.Repository;
            var total = await repo.CountAsync(
                query => {
                    if (model.RequestDate.HasValue) {
                        var startTime = model.RequestDate.Value;
                        var endTime = startTime.AddDays(1);
                        query = query.Where(
                            log => log.StartAt >= startTime && log.StartAt < endTime
                        );
                    }
                    if (model.UserName.IsNotNullOrEmpty()) {
                        query = query.Where(log => log.UserName.Contains(model.UserName));
                    }
                    return query;
                }
            );
            var data = await repo.QueryAsync(
                query => {
                    if (model.RequestDate.HasValue) {
                        var startTime = model.RequestDate.Value;
                        var endTime = startTime.AddDays(1);
                        query = query.Where(
                            log => log.StartAt >= startTime && log.StartAt < endTime
                        );
                    }
                    if (model.UserName.IsNotNullOrEmpty()) {
                        query = query.Where(log => log.UserName.Contains(model.UserName));
                    }
                    return query.OrderByDescending(log => log.Id)
                        .Skip(model.Skip)
                        .Take(model.Take);
                }
            );
            return new PaginatedResponseModel<AppAuditLogModel> {
                Total = total,
                Data = Mapper.Map<IList<AppAuditLogModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}
