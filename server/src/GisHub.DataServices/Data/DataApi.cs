using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据API</summary>
    [Class(Schema = "public", Table = "data_apis", Where = "is_deleted = false")]
    public partial class DataApi : BaseEntity<long> {

        /// <summary>数据API ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>数据API名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 32)]
        public virtual string Name { get; set; }

        /// <summary>数据API描述</summary>
        [Property(Name = "Description", Column = "description", Type = "string", NotNull = false, Length = 64)]
        public virtual string Description { get; set; }

        /// <summary>数据源ID</summary>
        [Property(Name = "DataSourceId", Column = "data_source_id", Type = "long", NotNull = true)]
        public virtual long DataSourceId { get; set; }

        /// <summary>是否向数据源写入数据</summary>
        [Property(Name = "WriteData", Column = "write_data", Type = "bool", NotNull = true)]
        public virtual bool WriteData { get; set; }

        /// <summary>数据API调用的 XML + SQL 命令</summary>
        [Property(Name = "Statement", Column = "statement", Type = "string", NotNull = true, Length = -1)]
        public virtual string Statement { get; set; }

        /// <summary>参数定义</summary>
        [Property(Name = "Parameters", Column = "parameters", Type = "string", NotNull = true, Length = -1)]
        public virtual string Parameters { get; set; }

        /// <summary>API 输出列的源数据</summary>
        [Property(Name = "Columns", Column = "columns", Type = "string", NotNull = false, Length = -1)]
        public virtual string Columns { get; set; }

        /// <summary>允许访问的角色</summary>
        [Property(Name = "Roles", Column = "roles", TypeType = typeof(NHibernate.Extensions.NpgSql.StringArrayType), NotNull = false)]
        public virtual string[] Roles { get; set; }

        /// <summary>创建者id</summary>
        [Property(Name = "CreatorId", Column = "creator_id", Type = "string", NotNull = true, Length = 32)]
        public virtual string CreatorId { get; set; }

        /// <summary>创建时间</summary>
        [Property(Name = "CreatedAt", Column = "created_at", Type = "datetime", NotNull = true)]
        public virtual DateTime CreatedAt { get; set; }

        /// <summary>更新者id</summary>
        [Property(Name = "UpdaterId", Column = "updater_id", Type = "string", NotNull = true, Length = 32)]
        public virtual string UpdaterId { get; set; }

        /// <summary>更新时间</summary>
        [Property(Name = "UpdatedAt", Column = "updated_at", Type = "datetime", NotNull = true)]
        public virtual DateTime UpdatedAt { get; set; }

        /// <summary>是否删除</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }
    }

}
