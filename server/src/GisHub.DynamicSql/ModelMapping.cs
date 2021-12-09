using System.Xml;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;

namespace Beginor.GisHub.DynamicSql {

    public class ModelMapping : AutoMapper.Profile {

        public ModelMapping() {
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