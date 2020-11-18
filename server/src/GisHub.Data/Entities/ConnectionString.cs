using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.Data.Entities {

    /// <summary>数据库连接串</summary>
    [Class(Schema = "public", Table = "connection_strings")]
    public partial class ConnectionString : BaseEntity<long> {

        /// <summary>连接串ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>连接串名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 64)]
        public virtual string Name { get; set; }

        /// <summary>连接串值</summary>
        [Property(Name = "Value", Column = "value", Type = "string", NotNull = true, Length = 512)]
        public virtual string Value { get; set; }

        /// <summary>数据库类型（postgres、mssql、mysql、oracle、sqlite等）</summary>
        [Property(Name = "DatabaseType", Column = "database_type", Type = "string", NotNull = true, Length = 16)]
        public virtual string DatabaseType { get; set; }

        /// <summary>是否已删除（软删除）</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }
    }

}
