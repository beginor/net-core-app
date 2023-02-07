using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices;

public class MarkdownServiceDocBuilder : IApiBuilder<DataServiceCacheItem> {

    private IDataServiceFactory factory;
    private ILogger<MarkdownServiceDocBuilder> logger;

    public MarkdownServiceDocBuilder(IDataServiceFactory factory, ILogger<MarkdownServiceDocBuilder> logger) {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string BuildApiDoc(DocModel<DataServiceCacheItem> model) {
        var doc = new StringBuilder();
        doc.AppendLine($"# {model.Title}");
        doc.AppendLine("");
        if (!string.IsNullOrEmpty(model.Description)) {
            doc.AppendLine(model.Description);
            doc.AppendLine("");
        }

        var sampleDataUrl = string.Empty;
        DataServiceCacheItem? sampleService = null;

        foreach (var service in model.Models) {
            doc.AppendLine($"## {service.DataServiceName}");
            doc.AppendLine("");
            doc.AppendLine($"{service.DataServiceName}, {service.DataServiceDescription}");
            doc.AppendLine("");

            BuildFieldTable(doc, service.Fields);

            doc.AppendLine("### 服务接口");
            doc.AppendLine("");

            var (dataUrl, distinctUrl, geoJsonUrl, mvtUrl) = BuildApiUrls(model.BaseUrl, service.DataServiceId);
            if (string.IsNullOrEmpty(sampleDataUrl)) {
                sampleDataUrl = dataUrl;
                sampleService = service;
            }

            BuildDataDoc(doc, service, dataUrl, model.Token, model.Referer);
            BuildDistinctDataDoc(doc, service, distinctUrl, model.Token, model.Referer);

            if (!string.IsNullOrEmpty(service.GeometryColumn)) {
                BuildGeoJsonDoc(doc, service, geoJsonUrl, model.Token, model.Referer);
                if (service.SupportMvt) {
                    BuildMvtDoc(doc, service, mvtUrl, model.Token, model.Referer);
                }
            }
        }

        if (!string.IsNullOrEmpty(sampleDataUrl) && sampleService != null) {
            BuildAttentionDoc(doc, sampleDataUrl, sampleService, model.Token, model.Referer);
        }
        return doc.ToString();
    }

    private void BuildAttentionDoc(StringBuilder doc, string dataUrl, DataServiceCacheItem service, string token, string? referer) {
        doc.AppendLine("## 注意问题");
        doc.AppendLine("");
        doc.AppendLine("### 访问凭证");
        doc.AppendLine("");
        doc.AppendLine("凭证参数需要向数据服务提供者申请。");
        doc.AppendLine("");
        doc.AppendLine("### 参数加密");
        doc.AppendLine("");
        doc.AppendLine("在请求 JSON/GeoJSON 格式数据时， 参数 `$select`, `$where` 等可以自定义参数值， 因此可能会触发防火墙规则导致无法请求， 如果出现这样的情况， 则需要进行先对参数值进行加密，参数名称不变。");
        doc.AppendLine("");
        doc.AppendLine("- `$token` 和 `$encrypted` 两个参数不加密；");
        doc.AppendLine("- 其它参数按照下面的说明进行加密；");
        doc.AppendLine("");
        doc.AppendLine("参数加密方法为 `url safe base64` ，目前各种主流语言均已经有现有的实现， 以 TypeScript 为例， 加密代码如下：");
        doc.AppendLine("");
        doc.AppendLine("``` typescript");
        doc.AppendLine("export function urlSafeEncode(input: string): string {");
        doc.AppendLine("    let output = btoa(input);");
        doc.AppendLine("    output = output.split('=')[0];");
        doc.AppendLine("    output = output.replace('+', '-');");
        doc.AppendLine("    output = output.replace('/', '_');");
        doc.AppendLine("    return output;");
        doc.AppendLine("}");
        doc.AppendLine("```");
        doc.AppendLine("");
        doc.AppendLine("参数加密前的请求示例:");
        doc.AppendLine("");
        var select = $"{service.PrimaryKeyColumn},{service.DisplayColumn}";
        var where = $"{service.DisplayColumn} is not null";
        var orderBy = service.PrimaryKeyColumn;
        doc.AppendLine("```http");
        doc.AppendLine($"GET {dataUrl}?$token={token}&$encrypted=false&$select={select}&$where={where}&$orderBy={orderBy}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
        doc.AppendLine("");
        doc.AppendLine("将参数加密后的请求示例：");
        doc.AppendLine("");
        doc.AppendLine("```http");
        doc.AppendLine($"GET {dataUrl}?$token={token}&$encrypted=true&$select={Base64UrlEncoder.Encode(select)}&$where={Base64UrlEncoder.Encode(where)}&$orderBy={Base64UrlEncoder.Encode(orderBy)}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
    }

    private void BuildDataDoc(StringBuilder doc, DataServiceCacheItem service, string dataUrl, string token, string? referer) {
        doc.AppendLine("#### 属性数据 (JSON)");
        doc.AppendLine("");
        doc.AppendLine("##### 地址");
        doc.AppendLine("");
        doc.AppendLine($"- <{dataUrl}>");
        doc.AppendLine("");
        doc.AppendLine("##### 请求方法");
        doc.AppendLine("");
        doc.AppendLine("- GET");
        doc.AppendLine("");
        doc.AppendLine("##### 参数");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 | 是否必须 |");
        doc.AppendLine("| :---- | :---- | :---- | :---- |");
        doc.AppendLine("| $token | string | 访问凭证 | 是 |");
        doc.AppendLine("| $encrypted | boolean | 参数是否加密，(在生产环境下，必须对请求参数进行加密， 加密方法参看后面的参数加密一节)。 | 是 |");
        doc.AppendLine("| $select | string | 要输出的字段，支持常见的SQL统计函数 (Count, Avg, Max, Min等)，参考 SQL 语法的 SELECT 语句 | 否 |");
        doc.AppendLine("| $where | string | 过滤条件，针对服务的输出字段进行自定义过滤，参考 SQL 语言的 WHERE 语句。 | 否 |");
        doc.AppendLine("| $groupBy | string | 分组， 当 $select 包含统计函数时，必须使用这个参数进行分组，参考 SQL 语言的 GROUP BY 语句。 | 否 |");
        doc.AppendLine("| $orderBy | string | 排序， 参考 SQL 语言的 ORDER BY 语句。 | 否 |");
        doc.AppendLine("| $skip | number | 分页， 跳过多少条记录，默认值为0。 | 否 |");
        doc.AppendLine("| $take | number | 分页，读取多少条记录，默认值为10。 | 否 |");
        doc.AppendLine("");
        doc.AppendLine("##### 输出描述");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 |");
        doc.AppendLine("| :-- | :-- | :-- |");
        doc.AppendLine("| total | number | 总的记录数。 |");
        doc.AppendLine("| data | object[] | 对象数组形式的结果数据。 |");
        doc.AppendLine("| take | number | 请求参数重要读区的记录数，如果小于data中的记录数，则表示已经读取完毕。 |");
        doc.AppendLine("| skip | number | 跳过的记录数。 |");
        doc.AppendLine("");
        doc.AppendLine("##### 请求示例");
        doc.AppendLine("");
        doc.AppendLine("```http");
        doc.AppendLine($"GET {dataUrl}?$token={token}&$encrypted=false&$select={service.PrimaryKeyColumn},{service.DisplayColumn}&$where={service.DisplayColumn} is not null&$orderBy={service.PrimaryKeyColumn}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
        doc.AppendLine("");
    }

    private void BuildDistinctDataDoc(StringBuilder doc, DataServiceCacheItem service, string distinctUrl, string token, string? referer) {
        doc.AppendLine("#### 非重复属性数据");
        doc.AppendLine("");
        doc.AppendLine("##### 地址");
        doc.AppendLine("");
        doc.AppendLine($"- <{distinctUrl}>");
        doc.AppendLine("");
        doc.AppendLine("##### 请求方法");
        doc.AppendLine("");
        doc.AppendLine("- GET");
        doc.AppendLine("");
        doc.AppendLine("##### 参数");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 | 是否必须 |");
        doc.AppendLine("| :---- | :---- | :---- | :---- |");
        doc.AppendLine("| $token | string | 访问凭证 | 是 |");
        doc.AppendLine("| $encrypted | boolean | 参数是否加密，(在生产环境下，必须对请求参数进行加密， 加密方法参看后面的参数加密一节)。 | 是 |");
        doc.AppendLine("| $select | string | 要输出的字段，支持常见的SQL统计函数 (Count, Avg, Max, Min等)，参考 SQL 语法的 SELECT 语句 | 否 |");
        doc.AppendLine("| $where | string | 过滤条件，针对服务的输出字段进行自定义过滤，参考 SQL 语言的 WHERE 语句。 | 否 |");
        doc.AppendLine("| $orderBy | string | 排序， 参考 SQL 语言的 ORDER BY 语句。 | 否 |");
        doc.AppendLine("");
        doc.AppendLine("##### 输出描述");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 |");
        doc.AppendLine("| :-- | :-- | :-- |");
        doc.AppendLine("| data | object[] | 对象数组形式的结果数据。 |");
        doc.AppendLine("");
        doc.AppendLine("##### 请求示例");
        doc.AppendLine("");
        doc.AppendLine("```http");
        doc.AppendLine($"GET {distinctUrl}?$token={token}&$encrypted=false&$select={service.DisplayColumn}&$where={service.DisplayColumn} is not null&$orderBy={service.DisplayColumn}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
        doc.AppendLine("");
    }

    private void BuildGeoJsonDoc(StringBuilder doc, DataServiceCacheItem service, string geoJsonUrl, string token, string? referer) {
        doc.AppendLine("#### 空间数据 (GeoJson)");
        doc.AppendLine("");
        doc.AppendLine("##### 地址");
        doc.AppendLine("");
        doc.AppendLine($"- <{geoJsonUrl}>");
        doc.AppendLine("");
        doc.AppendLine("##### 请求方法");
        doc.AppendLine("");
        doc.AppendLine("- GET");
        doc.AppendLine("");
        doc.AppendLine("##### 参数");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 | 是否必须 |");
        doc.AppendLine("| :--- | :--- | :--- | :---- |");
        doc.AppendLine("| $token | string | 访问凭证 | 是 |");
        doc.AppendLine("| $encrypted | boolean | 参数是否加密，(在生产环境下，必须对请求参数进行加密， 加密方法参看后面的参数加密一节)。 | 否 |");
        doc.AppendLine("| $select | string | 自定义要输出的字段，支持常见的SQL统计函数 (Count, Avg, Max, Min等)，参考 SQL 语法的 SELECT 语句 | 否 |");
        doc.AppendLine("| $where | string | 自定义查询过滤条件，针对服务的输出字段进行自定义过滤，参考 SQL 语言的 WHERE 语句。 | 否 |");
        doc.AppendLine("| $orderBy | string | 自定义排序， 参考 SQL 语言的 ORDER BY 语句。 | 否 |");
        doc.AppendLine("| $skip | number | 自定义分页， 跳过多少条记录，默认值为0。 | 否 |");
        doc.AppendLine("| $take | number | 自定义分页，读取多少条记录，默认值为10。 | 否 |");
        doc.AppendLine("");
        doc.AppendLine("##### 输出描述");
        doc.AppendLine("");
        doc.AppendLine("输出结果为符合标准 [GeoJSON](https://geojson.org/) 格式的 FeatureCollection 对象， 可以在 ArcGIS JS API、 MapboxGL、 OpenLayers 等各种支持 GeoJSON 格式的地图空间中使用， 也可以直接在 ArcGIS 、 QGIS 等专业软件中查看。");
        doc.AppendLine("");
        doc.AppendLine("##### 请求示例");
        doc.AppendLine("");
        doc.AppendLine("```http");
        doc.AppendLine($"GET {geoJsonUrl}?$token={token}&$encrypted=false&$select={service.PrimaryKeyColumn},{service.DisplayColumn},{service.GeometryColumn}&$where={service.GeometryColumn} is not null&orderBy={service.PrimaryKeyColumn}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
        doc.AppendLine("");
    }

    private void BuildMvtDoc(StringBuilder doc, DataServiceCacheItem service, string mvtUrl, string token, string? referer) {
        doc.AppendLine("#### 矢量切片");
        doc.AppendLine("");
        doc.AppendLine("##### 地址");
        doc.AppendLine("");
        doc.AppendLine($"- <{mvtUrl}>");
        doc.AppendLine("");
        doc.AppendLine("##### 请求方法");
        doc.AppendLine("");
        doc.AppendLine("- GET");
        doc.AppendLine("");
        doc.AppendLine("##### 参数");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 | 是否必须 |");
        doc.AppendLine("| :--- | :--- | :--- | :---- |");
        doc.AppendLine("| $token | string | 访问凭证 | 是 |");
        doc.AppendLine("");
        doc.AppendLine("##### 输出描述");
        doc.AppendLine("");
        doc.AppendLine("| 切片架构 | xyz  |");
        doc.AppendLine("| ---------- | ------- |");
        doc.AppendLine("| 坐标系 | Web 墨卡托 WGS3857 |");
        try {
            var featureProvider = factory.CreateFeatureProvider(service.DatabaseType);
            if (featureProvider != null) {
                var task = featureProvider.QueryForExtentAsync(
                    service,
                    new AgsQueryParam { OutSR = AgsSpatialReference.WGS84.Wkid }
                );
                task.Wait();
                var extent = task.Result.Extent;
                if (extent != null) {
                    doc.AppendLine($"| 经纬度范围 | [107.00000,19.00000,130.00000,26.00000] |");
                }
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not query extent for dataservice {service.DataServiceId} !");
        }
        doc.AppendLine($"| 最大级别 | {service.MvtMaxZoom} |");
        doc.AppendLine($"| 最小级别 | {service.MvtMinZoom} |");
        doc.AppendLine($"| 缓存时间 | {service.MvtCacheDuration} |");
        doc.AppendLine("");
        doc.AppendLine("##### 请求示例");
        doc.AppendLine("");
        doc.AppendLine("```http");
        doc.AppendLine($"GET {mvtUrl}?$token={token}");
        if (!string.IsNullOrEmpty(referer)) {
            doc.AppendLine($"Referer: {referer}");
        }
        doc.AppendLine("```");
    }

    internal static (string, string, string, string) BuildApiUrls(string baseUrl, long serviceId) {
        var dataUrl = $"{baseUrl}/{serviceId}/data";
        var distinctUrl = $"{baseUrl}/{serviceId}/distinct";
        var geoJsonUrl = $"{baseUrl}/{serviceId}/geojson";
        var mvtUrl = $"{baseUrl}/{serviceId}/mvt/{{z}}/{{y}}/{{x}}";
        return (dataUrl, distinctUrl, geoJsonUrl, mvtUrl);
    }

    private static void BuildFieldTable(StringBuilder doc, IEnumerable<DataServiceField> fields) {
        doc.AppendLine("### 输出字段");
        doc.AppendLine("");
        doc.AppendLine("| 名称 | 类型 | 说明 |");
        doc.AppendLine("| :-- | :-- | :-- |");
        foreach (var param in fields) {
            doc.AppendLine($"| {param.Name} | {param.Type} | {param.Description} |");
        }
        doc.AppendLine("");
    }

}
