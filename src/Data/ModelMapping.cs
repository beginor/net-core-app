using System;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data;

public class ModelMapping : AutoMapper.Profile {

    public ModelMapping() {
        CreateMap<AppRole, AppRoleModel>().ReverseMap();
        CreateMap<AppUser, AppUserModel>()
            .ReverseMap()
            .ForMember(dest => dest.DisplayName, map => map.MapFrom(src => src.Surname + src.GivenName));
        CreateMap<AppUser, StringIdNameEntity>()
            .ForMember(dest => dest.Name, map => map.MapFrom(src => src.DisplayName))
            .ReverseMap()
            .ForMember(dest => dest.UserName, map => map.Ignore());
        CreateMap<AppRole, StringIdNameEntity>().ReverseMap();
        // 添加其它的映射
        CreateMap<AppAttachment, AppAttachmentModel>()
            .ForMember(dest => dest.CreatorId, map => map.MapFrom(src => src.Creator.Id))
            .ForMember(dest => dest.CreatorName, map => map.MapFrom(src => src.Creator.DisplayName))
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
            .ForMember(dest => dest.User, map => map.Ignore());
        CreateMap<AppLog, AppLogModel>();
        CreateMap<AppOrganizeUnit, AppOrganizeUnitModel>()
            .ReverseMap();
        CreateMap<AppOrganizeUnit, StringIdNameEntity>()
            .ReverseMap();
    }

}
