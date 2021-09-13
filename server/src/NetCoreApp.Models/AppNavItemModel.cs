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
        /// <summary>角色</summary>
        public string[] Roles { get; set; }
        /// <summary>导航目标</summary>
        public string Target { get; set; }
        /// <summary>内嵌窗口地址</summary>
        public string FrameUrl { get; set; }
        /// <summary>是否删除</summary>
        public bool IsHidden { get; set; }
    }

    /// <summary>导航节点（菜单）搜索参数</summary>
    public partial class AppNavItemSearchModel : PaginatedRequestModel { }

}
