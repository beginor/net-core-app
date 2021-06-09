using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据服务</summary>
    [Class(Schema = "public", Table = "data_services", Where = "is_deleted = false")]
    public partial class DataService : BaseEntity<long> {

        /// <summary>数据服务id</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>数据服务名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 32)]
        public virtual string Name { get; set; }

        /// <summary>数据服务描述</summary>
        [Property(Name = "Description", Column = "description", Type = "string", NotNull = false, Length = 256)]
        public virtual string Description { get; set; }

        /// <summary>数据库连接id</summary>
        [ManyToOne(Name = "Connection", Column = "connection_id", ClassType = typeof(Connection), NotFound = NotFoundMode.Ignore)]
        public virtual Connection Connection { get; set; }

        /// <summary>数据表/视图架构</summary>
        [Property(Name = "Schema", Column = "schema", Type = "string", NotNull = false, Length = 16)]
        public virtual string Schema { get; set; }

        /// <summary>数据表/视图名称</summary>
        [Property(Name = "TableName", Column = "table_name", Type = "string", NotNull = true, Length = 64)]
        public virtual string TableName { get; set; }

        /// <summary>数据服务公开的字段列表</summary>
        [Property(Name = "Fields", Column = "fields", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataServiceField[]>), NotNull = false)]
        public virtual DataServiceField[] Fields { get; set; }

        /// <summary>主键列名称</summary>
        [Property(Name = "PrimaryKeyColumn", Column = "primary_key_column", Type = "string", NotNull = true, Length = 256)]
        public virtual string PrimaryKeyColumn { get; set; }

        /// <summary>显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。</summary>
        [Property(Name = "DisplayColumn", Column = "display_column", Type = "string", NotNull = true, Length = 256)]
        public virtual string DisplayColumn { get; set; }

        /// <summary>空间列</summary>
        [Property(Name = "GeometryColumn", Column = "geometry_column", Type = "string", NotNull = false, Length = 256)]
        public virtual string GeometryColumn { get; set; }

        /// <summary>预置过滤条件</summary>
        [Property(Name = "PresetCriteria", Column = "preset_criteria", Type = "string", NotNull = false, Length = 128)]
        public virtual string PresetCriteria { get; set; }

        /// <summary>默认排序</summary>
        [Property(Name = "DefaultOrder", Column = "default_order", Type = "string", NotNull = false, Length = 128)]
        public virtual string DefaultOrder { get; set; }

        /// <summary>标签</summary>
        [Property(Name = "Tags", Column = "tags", TypeType = typeof(NHibernate.Extensions.NpgSql.StringArrayType), NotNull = false)]
        public virtual string[] Tags { get; set; }

        /// <summary>是否删除</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }

        /// <summary>允许的角色</summary>
        [Property(Name = "Roles", Column = "roles", TypeType = typeof(NHibernate.Extensions.NpgSql.StringArrayType), NotNull = false)]
        public virtual string[] Roles { get; set; }

    }

    public class DataServiceField {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public bool Nullable { get; set; }
        public bool Editable { get; set; }
    }

}
