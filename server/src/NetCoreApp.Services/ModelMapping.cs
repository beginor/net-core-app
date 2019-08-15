using System;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<AppRole, AppRoleModel>().ReverseMap();
            CreateMap<AppUser, AppUserModel>().ReverseMap();
            CreateMap<AppUser, StringIdNameEntity>()
                .ForMember(
                    d => d.Name,
                    m => m.MapFrom(s => s.UserName)
                )
                .ReverseMap();
            CreateMap<AppRole, StringIdNameEntity>().ReverseMap();
            // 添加其它的映射
            CreateMap<AppAttachment, AppAttachmentModel>().ReverseMap();
            CreateMap<AppNavItem, AppNavItemModel>().ReverseMap();
        }

    }

}
