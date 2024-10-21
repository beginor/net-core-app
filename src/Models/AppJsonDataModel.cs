using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Beginor.AppFx.Core;

#nullable disable

namespace Beginor.NetCoreApp.Models;

/// <summary>json 数据模型</summary>
public partial class AppJsonDataModel : StringEntity {

    /// <summary>json值</summary>
    [Required(ErrorMessage = "json值必须填写！")]
    public JsonElement Value { get; set; }
    /// <summary>业务ID</summary>
    [Required(ErrorMessage = "业务ID必须填写！")]
    public string BusinessId { get; set; } = "0";
    /// <summary>名称</summary>
    [Required(ErrorMessage = "名称必须填写！")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>json 数据搜索参数</summary>
public partial class AppJsonDataSearchModel : PaginatedRequestModel {
    /// <summary>业务ID</summary>
    public long BusinessId { get; set; } = 0L;
    /// <summary>名称</summary>
    public string Name { get; set; } = string.Empty;
}
