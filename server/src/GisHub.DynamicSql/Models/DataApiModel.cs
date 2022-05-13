using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DynamicSql.Models; 

/// <summary>数据API模型</summary>
public partial class DataApiModel : BaseResourceModel {
    /// <summary>数据源</summary>
    [Required(ErrorMessage = "数据源 必须填写！")]
    public StringIdNameEntity DataSource { get; set; }
    /// <summary>是否向数据源写入数据</summary>
    [Required(ErrorMessage = "是否向数据源写入数据 必须填写！")]
    public bool WriteData { get; set; }
    /// <summary>数据API调用的 XML + SQL 命令</summary>
    [Required(ErrorMessage = "数据API调用的 XML + SQL 命令 必须填写！")]
    public string Statement { get; set; }
    /// <summary>参数定义</summary>
    [Required(ErrorMessage = "参数定义 必须填写！")]
    public DataApiParameterModel[] Parameters { get; set; }
    /// <summary>API 输出列的元数据</summary>
    public DataServiceFieldModel[] Columns { get; set; }
    /// <summary>输出字段中的标识列</summary>
    public string IdColumn { get; set; }
    /// <summary>输出字段中的空间列</summary>
    public string GeometryColumn { get; set; }
}

public class DataApiParameterModel {
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Source { get; set; }
    public bool Required { get; set; }
}

/// <summary>数据API搜索参数</summary>
public partial class DataApiSearchModel : PaginatedRequestModel {
    /// <summary>搜索关键字</summary>
    public string Keywords { get; set; }
}

/// <summary>数据API结果</summary>
public partial class DataApiResultModel {
    public IList<IDictionary<string, object>> Data { get; set; }
}
