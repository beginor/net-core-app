using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Models;

namespace Beginor.GisHub.DynamicSql;

public class MarkdownApiDocBuilder : IApiDocBuilder {

    public string BuildApiDoc(string pageTitle, string baseUrl, IEnumerable<DataApiModel> models, string token, string referer) {
        Argument.NotNullOrEmpty(pageTitle, nameof(pageTitle));
        Argument.NotNullOrEmpty(baseUrl, nameof(baseUrl));
        Argument.NotNullOrEmpty(models, nameof(models));
        Argument.NotNullOrEmpty(token, nameof(token));
        //
        var doc = new StringBuilder();
        doc.AppendLine($"# {pageTitle}");
        doc.AppendLine();
        foreach (var model in models) {
            BuildApiDocForModel(doc, baseUrl, model, token, referer);
        }
        return doc.ToString();
    }

    private (string, string) BuildApiUrls(string baseUrl, string apiId) {
        var jsonUrl = $"{baseUrl}/{apiId}/data";
        var geoJsonUrl = $"{baseUrl}/{apiId}/geojson";
        return (jsonUrl, geoJsonUrl);
    }

    private void BuildApiDocForModel(StringBuilder doc, string baseUrl, DataApiModel api, string token, string referer) {
        // title and description
        doc.AppendLine($"## {api.Name}");
        doc.AppendLine();
        doc.AppendLine(api.Description);
        doc.AppendLine();
        // url
        doc.AppendLine("### 地址");
        doc.AppendLine();
        var jsonUrl = $"{baseUrl}/{api.Id}/data";
        var geoJsonUrl = $"{baseUrl}/{api.Id}/geojson";
        doc.AppendLine($"- JSON 数据 <{jsonUrl}>");
        if (api.GeometryColumn.IsNotNullOrEmpty()) {
            doc.AppendLine($"- GeoJSON 数据 <{geoJsonUrl}>");
        }
        doc.AppendLine();
        // columns
        doc.AppendLine("### 输出字段");
        doc.AppendLine();
        doc.AppendLine("| 名称 | 类型 | 说明 |");
        doc.AppendLine("| :-- | :-- | :-- |");
        var geoColType = (DataServiceFieldModel f) => f.Name.EqualsOrdinalIgnoreCase(api.GeometryColumn) ? "空间坐标" : f.Type;
        foreach (var col in api.Columns) {
            doc.AppendLine($"| {col.Name} | {geoColType(col)} | {col.Description} |");
        }
        doc.AppendLine();
        // parameters
        doc.AppendLine("### 参数");
        doc.AppendLine();
        doc.AppendLine("| 名称 | 类型 | 说明 | 是否必须 |");
        doc.AppendLine("| :-- | :-- | :-- | :----- |");
        doc.AppendLine("| $token | string | 访问凭证 | 是 |");
        var yesOrNo = (bool required) =>  required ? "是" : "否";
        foreach (var param in api.Parameters) {
            doc.AppendLine($"| {param.Name} | {param.Type} | {param.Description} | {yesOrNo(param.Required)} |");
        }
        doc.AppendLine();
        // sample
        doc.AppendLine("### 请求示例");
        doc.AppendLine();
        doc.AppendLine("请求 JSON 格式数据");
        doc.AppendLine();
        doc.AppendLine("```http");
        doc.AppendLine($"{jsonUrl}?$token={token}&{api.Parameters[0].Name}=");
        if (referer.IsNotNullOrEmpty()) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
        doc.AppendLine();
        if (api.GeometryColumn.IsNotNullOrEmpty()) {
            doc.AppendLine("请求 GeoJSON 格式数据");
            doc.AppendLine();
            doc.AppendLine("```http");
            doc.AppendLine($"{geoJsonUrl}?$token=API_TOKEN&{api.Parameters[0].Name}=");
            doc.AppendLine("Referer: http://localhost:3000");
            doc.AppendLine("```");
            doc.AppendLine();
        }
        // attentions
        doc.AppendLine("> 注意问题：");
        doc.AppendLine(">");
        doc.AppendLine("> 1. 凭证参数需要向数据接口提供者申请。");
        doc.AppendLine("> 2. 数据接口暂时只支持使用 HTTP GET 方法请求，因此参数必须以 QueryString 的形式传递。");
        doc.AppendLine();
    }

}
