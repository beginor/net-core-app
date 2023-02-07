using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices; 

public class JsonServiceDocBuilder : IApiBuilder<DataServiceCacheItem>{

    public string BuildApiDoc(DocModel<DataServiceCacheItem> model) {
        var root = new JsonObject {
            ["$schema"] = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
            ["info"] = new JsonObject {
                ["_postman_id"] = Guid.NewGuid().ToString("D"),
                ["name"] = model.Title,
                ["description"] = model.Description ?? model.Title,
                ["schema"] = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
            },
            ["variable"] = new JsonArray {
                new JsonObject {
                    ["id"] = Guid.NewGuid().ToString("D"),
                    ["key"] = "$token",
                    ["value"] = model.Token,
                    ["type"] = "string"
                }
            }
        };
        var items = new JsonArray();
        root.Add("item", items);
        
        foreach (var service in model.Models) {
            var (dataUrl, distinctUrl, geoJsonUrl, mvtUrl) = MarkdownServiceDocBuilder.BuildApiUrls(model.BaseUrl, service.DataServiceId);
            var svcItem = new JsonObject {
                ["name"] = service.DataServiceName,
                ["description"] = service.DataServiceDescription
            };
            
            var svcItems = new JsonArray {
                BuildDataJsonDoc(service, dataUrl, model.Referer)
            };
            svcItem["item"] = svcItems;
            items.Add(svcItem);
        }
        return JsonSerializer.Serialize(
            root,
            new JsonSerializerOptions {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        );
    }
    
    private JsonObject BuildDataJsonDoc(DataServiceCacheItem service, string dataUrl, string? referer) {
        var dataUri = new Uri(dataUrl);
        var result = new JsonObject {
            ["name"] = "属性数据 (JSON)",
            ["request"] = new JsonObject {
                ["method"] = "GET",
                ["header"] = new JsonObject {
                    ["key"] = "Referer",
                    ["value"] = referer
                },
                ["url"] = new JsonObject {
                    ["protocol"] = dataUri.Scheme,
                    ["host"] = dataUri.Host,
                    ["port"] = dataUri.Port,
                    ["path"] = string.Join(string.Empty, dataUri.Segments),
                    ["query"] = new JsonArray {
                        new JsonObject {
                            ["key"] = "$token",
                            ["value"] = "{{$token}}",
                            ["description"] = "访问凭证",
                            ["disabled"] = false
                        },
                        new JsonObject {
                            ["key"] = "$encrypted",
                            ["value"] = "false",
                            ["description"] = "参数是否加密，(在生产环境下，必须对请求参数进行加密， 加密方法参看后面的参数加密一节)",
                            ["disabled"] = false
                        },
                        new JsonObject {
                            ["key"] = "$select",
                            ["value"] = $"{service.PrimaryKeyColumn},{service.DisplayColumn}",
                            ["description"] = "要输出的字段，支持常见的SQL统计函数 (Count, Avg, Max, Min等)，参考 SQL 语法的 SELECT 语句",
                            ["disabled"] = false
                        },
                        new JsonObject {
                            ["key"] = "$where",
                            ["value"] = $"{service.PrimaryKeyColumn} is not null",
                            ["description"] = "过滤条件，针对服务的输出字段进行自定义过滤，参考 SQL 语言的 WHERE 语句",
                            ["disabled"] = false
                        },
                        new JsonObject {
                            ["key"] = "$groupBy",
                            ["value"] = $"",
                            ["description"] = "分组， 当 $select 包含统计函数时，必须使用这个参数进行分组，参考 SQL 语言的 GROUP BY 语句",
                            ["disabled"] = true
                        },
                        new JsonObject {
                            ["key"] = "$orderBy",
                            ["value"] = $"",
                            ["description"] = "排序， 参考 SQL 语言的 ORDER BY 语句",
                            ["disabled"] = true
                        },
                        new JsonObject {
                            ["key"] = "$skip",
                            ["value"] = $"0",
                            ["description"] = "分页， 跳过多少条记录，默认值为0",
                            ["disabled"] = true
                        },
                        new JsonObject {
                            ["key"] = "$orderBy",
                            ["value"] = $"100",
                            ["description"] = "分页， 跳过多少条记录，默认值为0",
                            ["disabled"] = true
                        }
                    }
                }
            }
        };
        return result;
    }

}
