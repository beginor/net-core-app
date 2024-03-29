﻿using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

#nullable disable
namespace Beginor.NetCoreApp.Models;

/// <summary>组织单元模型</summary>
public partial class AppOrganizeUnitModel : StringEntity {
    /// <summary>上级组织单元 ID</summary>
    public string ParentId { get; set; }
    /// <summary>组织单元名称</summary>
    [Required(ErrorMessage = "组织单元名称 必须填写！")]
    public string Name { get; set; }
    /// <summary>组织单元说明</summary>
    public string Description { get; set; }
    /// <summary>组织机构排序</summary>
    [Required(ErrorMessage = "组织机构排序 必须填写！")]
    public float Sequence { get; set; }
    /// <summary>组织机构级别</summary>
    public int Level { get; set; } = 0;
    /// <summary>是否展开</summary>
    public bool Expand { get; set; } = false;
}

/// <summary>组织单元搜索参数</summary>
public partial class AppOrganizeUnitSearchModel : PaginatedRequestModel {
    /// <summary>组织机构ID</summary>
    public long? OrganizeUnitId { get; set; }
}
