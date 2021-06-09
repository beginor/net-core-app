using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<Connection, ConnectionModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<Connection, StringIdNameEntity>()
                .ReverseMap();
            CreateMap<DataService, DataServiceModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<DataServiceField, DataServiceFieldModel>()
                .ReverseMap();
        }

    }

}
