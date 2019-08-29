using System;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>系统权限</summary>
    public partial class AppPrivilege : BaseEntity<long>  {

            /// <summary>权限模块</summary>
        public virtual string Module { get; set; }
        /// <summary>权限名称( Identity 的策略名称)</summary>
        public virtual string Name { get; set; }
        /// <summary>权限描述</summary>
        public virtual string Description { get; set; }
        /// <summary>是否必须。 与代码中的 Authorize 标记对应的权限为必须的权限， 否则为可选的。</summary>
        public virtual bool IsRequired { get; set; }

    }

}
