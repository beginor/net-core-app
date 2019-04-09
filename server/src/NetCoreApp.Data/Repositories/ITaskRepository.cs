using System;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    public interface ITaskRepository : IRepository<Task, long> {

    }

}
