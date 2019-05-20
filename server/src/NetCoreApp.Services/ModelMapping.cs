using System;
using AutoMapper;
using Beginor.AppFx.Core;
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
                config.CreateMap<AppRole, AppRoleModel>().ReverseMap();
                config.CreateMap<AppUser, AppUserModel>().ReverseMap();
                config.CreateMap<AppUser, StringIdNameEntity>()
                    .ForMember(
                        d => d.Name,
                        m => m.MapFrom(s => s.UserName)
                    )
                    .ReverseMap();
                config.CreateMap<AppRole, StringIdNameEntity>().ReverseMap();
                // 添加其它的映射
                // 初始化完成
                Initialized = true;
            });
        }

    }

}
