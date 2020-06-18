using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.Models {

    /// <summary>系统权限模型</summary>
    public partial class AppPrivilegeModel : StringEntity {

        /// <summary>权限模块</summary>
        [Required(ErrorMessage = "权限模块 必须填写！")]
        public string Module { get; set; }
        /// <summary>权限名称( Identity 的策略名称)</summary>
        [Required(ErrorMessage = "权限名称( Identity 的策略名称) 必须填写！")]
        public string Name { get; set; }
        /// <summary>权限描述</summary>
        public string Description { get; set; }
        /// <summary>是否必须。 与代码中的 Authorize 标记对应的权限为必须的权限， 否则为可选的。</summary>
        [Required(ErrorMessage = "是否必须。 与代码中的 Authorize 标记对应的权限为必须的权限， 否则为可选的。 必须填写！")]
        public bool IsRequired { get; set; }

    }

    /// <summary>系统权限搜索参数</summary>
    public partial class AppPrivilegeSearchModel : PaginatedRequestModel {
        /// <summary>模块</summary>
        public string Module { get; set; }
    }

}
