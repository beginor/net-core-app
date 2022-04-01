using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models;

/// <summary>数据类别模型</summary>
public partial class CategoryModel : StringEntity {

    /// <summary>类别名称</summary>
    [Required(ErrorMessage = "类别名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>父类ID</summary>
    public string ParentId { get; set; }
    /// <summary>顺序号</summary>
    [Required(ErrorMessage = "顺序号 必须填写！")]
    public float Sequence { get; set; }

}

/// <summary>数据类别搜索参数</summary>
public partial class CategorySearchModel : PaginatedRequestModel { }
