using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>导航节点（菜单）</summary>
    public partial class AppNavItem : BaseEntity<long>  {

        /// <summary>parent_id</summary>
        public virtual long? ParentId { get; set; }
        /// <summary>标题</summary>
        public virtual string Title { get; set; }
        /// <summary>提示文字</summary>
        public virtual string Tooltip { get; set; }
        /// <summary>图标</summary>
        public virtual string Icon { get; set; }
        /// <summary>导航地址</summary>
        public virtual string Url { get; set; }
        /// <summary>顺序</summary>
        public virtual float? Sequence { get; set; }
        /// <summary>创建者ID</summary>
        public virtual string CreatorId { get; set; }
        /// <summary>创建时间</summary>
        public virtual DateTime CreatedAt { get; set; }
        /// <summary>更新者ID</summary>
        public virtual string UpdaterId { get; set; }
        /// <summary>更新时间</summary>
        public virtual DateTime UpdateAt { get; set; }
        /// <summary>是否删除</summary>
        public virtual bool IsDeleted { get; set; }

    }

}
