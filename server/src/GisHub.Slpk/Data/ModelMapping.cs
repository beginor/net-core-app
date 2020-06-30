using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Slpk.Data {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<SlpkEntity, SlpkModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }

    }

}
