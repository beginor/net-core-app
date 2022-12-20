using System;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models;

/// <summary>数据资源的基类模型</summary>
public partial class BaseResourceModel : StringEntity {
    /// <summary>资源名称</summary>
    public string Name { get; set; }
    /// <summary>资源描述</summary>
    public string Description { get; set; }
    /// <summary>资源类别</summary>
    public StringIdNameEntity Category { get; set; }
    /// <summary>允许访问的角色</summary>
    public string[] Roles { get; set; }
    /// <summary>资源标签</summary>
    public string[] Tags { get; set; }
    /// <summary>创建者</summary>
    public StringIdNameEntity Creator { get; set; }
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>更新者</summary>
    public StringIdNameEntity Updater { get; set; }
    /// <summary>更新时间</summary>
    public DateTime UpdatedAt { get; set; }
    /// <summary>资源类型</summary>
    public string Type { get; set; }
}

/// <summary>数据资源的基类搜索参数</summary>
public partial class BaseResourceSearchModel : PaginatedRequestModel {
    /// <summary>类别ID</summary>
    public long? CategoryId { get; set; }
    /// <summary>关键字</summary>
    public string? Keywords { get; set; }
}

/// <summary>数据资源的统计参数</summary>
public class BaseResourceStatisticRequestModel {
    /// <summary>资源类型</summary>
    public string Type { get; set; }
}

/// <summary>数据资源按类别统计</summary>
public class CategoryCountModel {
    /// <summary>类别ID</summary>
    public string CategoryId { get; set; }
    /// <summary>类别名称</summary>
    public string CategoryName { get; set; }
    /// <summary>数据资源数量</summary>
    public int Count { get; set; }
}
