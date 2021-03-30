using Beginor.GisHub.TileMap.Data;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<TileMapEntity, TileMapModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
            CreateMap<VectorTileEntity, VectorTileModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }

    }

}
