using System.Xml;
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
                .ForMember(dest => dest.Statement, map => map.MapFrom(src => src.Statement.OuterXml))
                .ReverseMap()
                .ForMember(dest => dest.Id, map => map.Ignore())
                .ForMember(dest => dest.Statement,map => map.MapFrom(src => StringToXmlDoc(src.Statement)));
            CreateMap<DataApiParameter, DataApiParameterModel>()
                .ReverseMap();
        }

        private static XmlDocument StringToXmlDoc(string xml) {
            var xmlDoc = new XmlDocument();
            if (!string.IsNullOrEmpty(xml)) {
                xmlDoc.LoadXml(xml);
            }
            return xmlDoc;
        }

    }

}
