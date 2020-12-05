using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<ConnectionString, ConnectionStringModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<DataSource, DataSourceModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }

    }

}
