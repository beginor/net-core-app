using System.Xml;
using AutoMapper;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;

namespace Beginor.GisHub.DynamicSql; 

public class ModelMapping : AutoMapper.Profile {

    public ModelMapping() {
        CreateMap<DataApi, DataApiModel>()
            .ForMember(dest => dest.Statement, map => map.MapFrom(src => src.Statement.OuterXml))
            .ReverseMap()
            .ForMember(dest => dest.Id, map => map.Ignore())
            // .ForMember(dest => dest.Statement, opt => opt.ConvertUsing(new StringToXmlConverter(), src => src.Statement));
            .ForMember(dest => dest.Statement, map => map.Ignore())
            .AfterMap((apiModel, api) => {
                    api.Statement = StringToXmlDoc(apiModel.Statement);
                    api.Type = "data_api";
                }
            );
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

public class StringToXmlConverter : AutoMapper.IValueConverter<string, XmlDocument> {

    public XmlDocument Convert(string sourceMember, ResolutionContext context) {
        var xmlDoc = new XmlDocument();
        if (!string.IsNullOrEmpty(sourceMember)) {
            xmlDoc.LoadXml(sourceMember);
        }
        return xmlDoc;
    }

}
