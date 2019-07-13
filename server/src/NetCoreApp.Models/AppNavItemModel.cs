using System;
using System.ComponentModel.DataAnnotations;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Models {

    /// <summary>导航节点（菜单）模型</summary>
    public partial class AppNavItemModel : StringEntity {

        /// <summary>parent_id</summary>
        public string ParentId { get; set; }
        /// <summary>标题</summary>
        [Required(ErrorMessage = "标题 必须填写！")]
        public string Title { get; set; }
        /// <summary>提示文字</summary>
        public string Tooltip { get; set; }
        /// <summary>图标</summary>
        public string Icon { get; set; }
        /// <summary>导航地址</summary>
        public string Url { get; set; }
        /// <summary>顺序</summary>
        public float? Sequence { get; set; }
        /// <summary>创建者ID</summary>
        [Required(ErrorMessage = "创建者ID 必须填写！")]
        public string CreatorId { get; set; }
        /// <summary>创建时间</summary>
        [Required(ErrorMessage = "创建时间 必须填写！")]
        public DateTime CreatedAt { get; set; }
        /// <summary>更新者ID</summary>
        [Required(ErrorMessage = "更新者ID 必须填写！")]
        public string UpdaterId { get; set; }
        /// <summary>更新时间</summary>
        [Required(ErrorMessage = "更新时间 必须填写！")]
        public DateTime UpdateAt { get; set; }
        /// <summary>是否删除</summary>
        [Required(ErrorMessage = "是否删除 必须填写！")]
        public bool IsDeleted { get; set; }

    }

    /// <summary>导航节点（菜单）搜索参数</summary>
    public partial class AppNavItemSearchModel : PaginatedRequestModel { }

}
