using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Models;

namespace Beginor.GisHub.DynamicSql;

public class JsonApiDocBuilder : IApiDocBuilder {

    public string BuildApiDoc(
        string title,
        string description,
        string baseUrl,
        IEnumerable<DataApiModel> models,
        string token,
        string referer
    ) {
        Argument.NotNullOrEmpty(title, nameof(title));
        Argument.NotNullOrEmpty(baseUrl, nameof(baseUrl));
        Argument.NotNullOrEmpty(models, nameof(models));
        Argument.NotNullOrEmpty(token, nameof(token));

        var root = new JsonObject {
            ["$schema"] = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
            ["info"] = new JsonObject {
                ["_postman_id"] = Guid.NewGuid().ToString("D"),
                ["name"] = title,
                ["description"] = description.IsNullOrEmpty() ? title : description,
                ["schema"] = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
            },
            ["variable"] = new JsonArray {
                new JsonObject {
                    ["id"] = Guid.NewGuid().ToString("D"),
                    ["key"] = "$token",
                    ["value"] = token,
                    ["type"] = "string"
                }
            }
        };
        var items = new JsonArray();
        foreach (var model in models) {
            var jsonUri = new Uri($"{baseUrl}/{model.Id}/data");
            var jsonItem = BuildJsonDocForModel(model, jsonUri, referer);
            items.Add(jsonItem);
            if (model.GeometryColumn.IsNotNullOrEmpty()) {
                var geojsonUrl = new Uri($"{baseUrl}/{model.Id}/geojson");
                model.Name += " (GeoJson)";
                var geojsonItem = BuildJsonDocForModel(model, geojsonUrl, referer);
                items.Add(geojsonItem);
            }
        }
        root.Add("item", items);
        return JsonSerializer.Serialize(
            root,
            new JsonSerializerOptions { WriteIndented = true }
        );
    }

    private JsonObject BuildJsonDocForModel(DataApiModel model, Uri uri, string referer) {
        return new JsonObject {
            ["id"] = Guid.NewGuid().ToString("D"),
            ["name"] = model.Name,
            ["request"] = new JsonObject {
                ["method"] = model.WriteData ? "POST" : "GET",
                ["header"] = new JsonArray {
                    new JsonObject {
                        ["key"] = "Referer",
                        ["value"] = referer
                    }
                },
                ["url"] = new JsonObject {
                    ["protocol"] = uri.Scheme,
                    ["host"] = uri.Host,
                    ["port"] = uri.Port,
                    ["path"] = string.Join(string.Empty, uri.Segments),
                    ["query"] = BuildJsonDocForParameters(model.Parameters)
                },
                ["description"] = new JsonObject {
                    ["type"] = "text/html",
                    ["content"] =
                        $"<p>{model.Description}</p><p>输出字段列表</p>{BuildOutputFieldTable(model.Columns, model.GeometryColumn)}"
                }
            }
        };
    }

    private JsonArray BuildJsonDocForParameters(IEnumerable<DataApiParameterModel> parameters) {
        var result = new JsonArray {
            new JsonObject {
                ["key"] = "$token",
                ["value"] = "{{$token}}",
                ["disabled"] = false
            }
        };
        foreach (var param in parameters) {
            var item = new JsonObject {
                ["key"] = param.Name,
                ["value"] = string.Empty,
                ["description"] = string.Format("{{{0}}} {1}", param.Type, param.Description),
                ["disabled"] = !param.Required
            };
            result.Add(item);
        }
        return result;
    }

    private string BuildOutputFieldTable(IEnumerable<DataServiceFieldModel> columns, string geometryColumn) {
        var html = new StringBuilder();
        html.Append("<table border='1'>");
        html.Append("<thead>");
        html.Append("<tr>");
        html.Append("<th>名称</th>");
        html.Append("<th>类型</th>");
        html.Append("<th>说明</th>");
        html.Append("</tr>");
        html.Append("<tbody>");
        var geoColType = (DataServiceFieldModel f) => f.Name.EqualsOrdinalIgnoreCase(geometryColumn) ? "空间坐标" : f.Type;
        foreach (var col in columns) {
            html.Append("<tr>");
            html.Append($"<td>{col.Name}</td>");
            html.Append($"<td>{geoColType(col)}</td>");
            html.Append($"<td>{col.Description}</td>");
            html.Append("</tr>");
        }
        html.Append("</tbody>");
        html.Append("</thead>");
        html.Append("</table>");
        return html.ToString();
    }

}
