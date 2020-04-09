using System;
using Beginor.AppFx.Core;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>导航节点（菜单）</summary>
    [Class(Schema = "public", Table = "app_nav_items")]
    public partial class AppNavItem : BaseEntity<long> {

        /// <summary>导航节点（菜单）ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id {
            get { return base.Id; }
            set { base.Id = value; }
        }
        /// <summary>父节点ID</summary>
        [Property(Name = "ParentId", Column = "parent_id", Type = "long", NotNull = true)]
        public virtual long? ParentId { get; set; }
        /// <summary>标题</summary>
        [Property(Name = "Title", Column = "title", Type = "string", NotNull = true, Length = 16)]
        public virtual string Title { get; set; }
        /// <summary>提示文字</summary>
        [Property(Name = "Tooltip", Column = "tooltip", Type = "string", NotNull = false, Length = 64)]
        public virtual string Tooltip { get; set; }
        /// <summary>图标</summary>
        [Property(Name = "Icon", Column = "icon", Type = "string", NotNull = false, Length = 32)]
        public virtual string Icon { get; set; }
        /// <summary>导航地址</summary>
        [Property(Name = "Url", Column = "url", Type = "string", NotNull = false, Length = 256)]
        public virtual string Url { get; set; }
        /// <summary>顺序</summary>
        [Property(Name = "Sequence", Column = "sequence", Type = "float", NotNull = true)]
        public virtual float? Sequence { get; set; }
        /// <summary>创建者ID</summary>
        [ManyToOne(Name = "Creator", Column = "creator_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
        public virtual AppUser Creator { get; set; }
        /// <summary>创建时间</summary>
        [Property(Name = "CreatedAt", Column = "created_at", Type = "datetime", NotNull = true)]
        public virtual DateTime CreatedAt { get; set; }
        /// <summary>更新者ID</summary>
        [ManyToOne(Name = "Updater", Column = "updater_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
        public virtual AppUser Updater { get; set; }
        /// <summary>更新时间</summary>
        [Property(Name = "UpdatedAt", Column = "updated_at", Type = "datetime", NotNull = true)]
        public virtual DateTime UpdatedAt { get; set; }
        /// <summary>是否删除</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }
        /// <summary>菜单项的角色</summary>
        [Property(Name = "Roles", Column = "roles", TypeType = typeof(StringArrayType), NotNull = false)]
        public virtual string[] Roles { get; set; }
        /// <summary>导航目标窗口</summary>
        [Property(Name = "Target", Column = "target", Type = "string", NotNull = false, Length = 16)]
        public virtual string Target { get; set; }
    }

}
