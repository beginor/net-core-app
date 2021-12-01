using System;
using System.Xml;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.DynamicSql.Data {

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
        [ManyToOne(Name = "DataSource", Column = "data_source_id", ClassType = typeof(DataSource), NotFound = NotFoundMode.Ignore)]
        public virtual DataSource DataSource { get; set; }

        /// <summary>是否向数据源写入数据</summary>
        [Property(Name = "WriteData", Column = "write_data", Type = "bool", NotNull = true)]
        public virtual bool WriteData { get; set; }

        /// <summary>数据API调用的 XML + SQL 命令</summary>
        [Property(Name = "Statement", Column = "statement", Type = "xml", NotNull = true)]
        public virtual XmlDocument Statement { get; set; }

        /// <summary>参数定义</summary>
        [Property(Name = "Parameters", Column = "parameters", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataApiParameter[]>), NotNull = false)]
        public virtual DataApiParameter[] Parameters { get; set; }

        /// <summary>API 输出列的源数据</summary>
        [Property(Name = "Columns", Column = "columns", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataServiceField[]>), NotNull = false)]
        public virtual DataServiceField[] Columns { get; set; }

        /// <summary>允许访问的角色</summary>
        [Property(Name = "Roles", Column = "roles", TypeType = typeof(NHibernate.Extensions.NpgSql.StringArrayType), NotNull = false)]
        public virtual string[] Roles { get; set; }

        /// <summary>创建者id</summary>
        [ManyToOne(Name = "Creator", Column = "creator_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
        public virtual AppUser Creator { get; set; }

        /// <summary>创建时间</summary>
        [Property(Name = "CreatedAt", Column = "created_at", Type = "datetime", NotNull = true)]
        public virtual DateTime CreatedAt { get; set; }

        /// <summary>更新者id</summary>
        [ManyToOne(Name = "Updater", Column = "updater_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
        public virtual AppUser Updater { get; set; }

        /// <summary>更新时间</summary>
        [Property(Name = "UpdatedAt", Column = "updated_at", Type = "datetime", NotNull = true)]
        public virtual DateTime UpdatedAt { get; set; }

        /// <summary>是否删除</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }
    }

    public class DataApiParameter {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public bool Required { get; set; }
    }

}
