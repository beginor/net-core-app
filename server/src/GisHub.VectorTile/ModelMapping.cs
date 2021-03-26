using System;
using Beginor.GisHub.VectorTile.Data;
using Beginor.GisHub.VectorTile.Models;

namespace Beginor.GisHub.VectorTile {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
            CreateMap<Vectortile, VectortileModel>()
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore());
        }
    }

}
