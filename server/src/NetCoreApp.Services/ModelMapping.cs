using System;
using AutoMapper;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public static class ModelMapping {

        public static void Setup() {
            Mapper.Initialize(config => {
                config.CreateMap<ApplicationRole, ApplicationRoleModel>().ReverseMap();
                config.CreateMap<ApplicationUser, ApplicationUserModel>().ReverseMap();
                // 添加其它的映射
            });
        }

    }

}
