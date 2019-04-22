using System;
using AutoMapper;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public static class ModelMapping {

        public static volatile bool Initialized;

        public static void Setup() {
            if (Initialized) {
                return;
            }
            Mapper.Initialize(config => {
                config.CreateMap<AppRole, ApplicationRoleModel>().ReverseMap();
                config.CreateMap<AppUser, ApplicationUserModel>().ReverseMap();
                // 添加其它的映射
                // 初始化完成
                Initialized = true;
            });
        }

    }

}
