using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public interface IUserService
        : IBaseService<ApplicationUserModel> {
    }

}
