using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>应用程序角色模型</summary>
    public class ApplicationRoleModel : StringEntity {

        /// <summary>角色名称</summary>
        [Required(ErrorMessage = "角色名称必须填写")]
        public string Name { get; set; }

        /// <summary>角色描述</summary>
        public string Description { get; set; }

    }

    public class RoleSearchRequestModel : PaginatedRequestModel {
        public string Name { get; set; }
    }

}
