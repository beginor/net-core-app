using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models;

#nullable disable

/// <summary>json 数据模型</summary>
public partial class AppJsonDataModel : StringEntity {

    /// <summary>json值</summary>
    [Required(ErrorMessage = "json值 必须填写！")]
    public string Value { get; set; }

}

/// <summary>json 数据搜索参数</summary>
public partial class AppJsonDataSearchModel : PaginatedRequestModel { }
