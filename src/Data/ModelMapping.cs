using System;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data;

public class ModelMapping : AutoMapper.Profile {

    public ModelMapping() {
        CreateMap<AppRoleEntity, AppRoleModel>().ReverseMap();
        CreateMap<AppUserEntity, AppUserModel>()
            .ReverseMap()
            .ForMember(dest => dest.DisplayName, map => map.MapFrom(src => src.Surname + src.GivenName));
        CreateMap<AppUserEntity, StringIdNameEntity>()
            .ForMember(dest => dest.Name, map => map.MapFrom(src => src.DisplayName))
            .ReverseMap()
            .ForMember(dest => dest.UserName, map => map.Ignore());
        CreateMap<AppRoleEntity, StringIdNameEntity>().ReverseMap();
        CreateMap<AppJsonDataEntity, AppJsonDataModel>()
            .ReverseMap();
        CreateMap<AppAttachmentEntity, AppAttachmentModel>()
            .ForMember(dest => dest.CreatorId, map => map.MapFrom(src => src.Creator.Id))
            .ForMember(dest => dest.CreatorName, map => map.MapFrom(src => src.Creator.DisplayName))
            .ReverseMap()
            .ForMember(dest => dest.Creator, map => map.Ignore())
            .ForMember(dest => dest.CreatedAt, map => map.Ignore());
        CreateMap<AppNavItemEntity, AppNavItemModel>()
            .ReverseMap();
        CreateMap<AppPrivilegeEntity, AppPrivilegeModel>()
            .ReverseMap();
        CreateMap<AppAuditLogEntity, AppAuditLogModel>()
            .ReverseMap();
        CreateMap<AppClientErrorEntity, AppClientErrorModel>()
            .ReverseMap();
        CreateMap<AppStorageEntity, AppStorageModel>()
            .ReverseMap();
        CreateMap<AppUserTokenEntity, AppUserTokenModel>()
            .ReverseMap()
            .ForMember(dest => dest.User, map => map.Ignore());
        CreateMap<AppLogEntity, AppLogModel>();
        CreateMap<AppOrganizeUnitEntity, AppOrganizeUnitModel>()
            .ReverseMap();
        CreateMap<AppOrganizeUnitEntity, StringIdNameEntity>()
            .ReverseMap();
    }

}
