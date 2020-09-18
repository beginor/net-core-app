using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Data {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<TileMapEntity, TileMapModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }

    }

}
