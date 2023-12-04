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
            .ForMember(dest => dest.Id, map => map.Ignore())
            .ForMember(dest => dest.Creator, map => map.Ignore())
            .ForMember(dest => dest.CreatedAt, map => map.Ignore());
        CreateMap<AppNavItem, AppNavItemModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
        CreateMap<AppPrivilege, AppPrivilegeModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
        CreateMap<AppAuditLog, AppAuditLogModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
        CreateMap<AppClientError, AppClientErrorModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
        CreateMap<AppStorage, AppStorageModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
        CreateMap<AppUserToken, AppUserTokenModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore())
            .ForMember(d => d.User, m => m.Ignore());
        CreateMap<AppLog, AppLogModel>();
        CreateMap<AppOrganizeUnit, AppOrganizeUnitModel>()
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore());
    }

}
