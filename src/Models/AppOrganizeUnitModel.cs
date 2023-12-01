using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

#nullable disable
namespace Beginor.NetCoreApp.Models;

/// <summary>组织单元模型</summary>
public partial class AppOrganizeUnitModel : StringEntity {

    /// <summary>上级组织单元 ID</summary>
    public string ParentId { get; set; }
    /// <summary>组织单元编码</summary>
    [Required(ErrorMessage = "组织单元编码 必须填写！")]
    public string Code { get; set; }
    /// <summary>组织单元名称</summary>
    [Required(ErrorMessage = "组织单元名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>组织单元说明</summary>
    public string Description { get; set; }
    /// <summary>组织机构排序</summary>
    [Required(ErrorMessage = "组织机构排序 必须填写！")]
    public float Sequence { get; set; }
    /// <summary>创建者id</summary>
    [Required(ErrorMessage = "创建者id 必须填写！")]
    public string CreatorId { get; set; }
    /// <summary>创建时间</summary>
    [Required(ErrorMessage = "创建时间 必须填写！")]
    public DateTime CreatedAt { get; set; }
    /// <summary>更新者id</summary>
    [Required(ErrorMessage = "更新者id 必须填写！")]
    public string UpdaterId { get; set; }
    /// <summary>更新时间</summary>
    [Required(ErrorMessage = "更新时间 必须填写！")]
    public DateTime UpdatedAt { get; set; }
    /// <summary>是否删除</summary>
    [Required(ErrorMessage = "是否删除 必须填写！")]
    public bool IsDeleted { get; set; }
}

/// <summary>组织单元搜索参数</summary>
public partial class AppOrganizeUnitSearchModel : PaginatedRequestModel { }
