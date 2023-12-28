using System;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data;

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
        CreateMap<AppAttachment, AppAttachmentModel>()
            .ForMember(
                dest => dest.CreatorId,
                map => map.MapFrom(src => src.Creator.Id)
            )
            .ForMember(
                dest => dest.CreatorName,
                map => map.MapFrom(src => src.Creator.UserName)
            )
            .ReverseMap()
            .ForMember(dest => dest.Creator, map => map.Ignore())
            .ForMember(dest => dest.CreatedAt, map => map.Ignore());
        CreateMap<AppNavItem, AppNavItemModel>()
            .ReverseMap();
        CreateMap<AppPrivilege, AppPrivilegeModel>()
            .ReverseMap();
        CreateMap<AppAuditLog, AppAuditLogModel>()
            .ReverseMap();
        CreateMap<AppClientError, AppClientErrorModel>()
            .ReverseMap();
        CreateMap<AppStorage, AppStorageModel>()
            .ReverseMap();
        CreateMap<AppUserToken, AppUserTokenModel>()
            .ReverseMap()
            .ForMember(d => d.User, m => m.Ignore());
        CreateMap<AppLog, AppLogModel>();
        CreateMap<AppOrganizeUnit, AppOrganizeUnitModel>()
            .ReverseMap();
        CreateMap<AppOrganizeUnit, StringIdNameEntity>()
            .ReverseMap();
    }

}
