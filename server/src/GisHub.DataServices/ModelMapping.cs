using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<DataSource, DataSourceModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<DataSource, StringIdNameEntity>()
                .ReverseMap();
            CreateMap<DataService, DataServiceModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<DataServiceField, DataServiceFieldModel>()
                .ReverseMap();
            CreateMap<DataApi, DataApiModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }

    }

}
