using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据源</summary>
    [Class(Schema = "public", Table = "data_sources", Where = "is_deleted = false")]
    public partial class DataSource : BaseEntity<long> {

        /// <summary>数据源ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>数据源名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 64)]
        public virtual string Name { get; set; }

        /// <summary>数据库类型（postgres、mssql、mysql、oracle、sqlite等）</summary>
        [Property(Name = "DatabaseType", Column = "database_type", Type = "string", NotNull = true, Length = 16)]
        public virtual string DatabaseType { get; set; }

        /// <summary> 服务器地址 </summary>
        [Property(Name = "ServerAddress", Column = "server_address", Type = "string", NotNull = true, Length = 64)]
        public virtual string ServerAddress { get; set; }

        /// <summary> 服务器端口 </summary>
        [Property(Name = "ServerPort", Column = "server_port", Type = "int", NotNull = false)]
        public virtual int ServerPort { get; set; }

        /// <summary> 数据库名称 </summary>
        [Property(Name = "DatabaseName", Column = "database_name", Type = "string", NotNull = true, Length = 64)]
        public virtual string DatabaseName { get; set; }

        /// <summary> 用户名 </summary>
        [Property(Name = "Username", Column = "username", Type = "string", NotNull = false, Length = 64)]
        public virtual string Username { get; set; }

        /// <summary> 密码 </summary>
        [Property(Name = "Password", Column = "password", Type = "string", NotNull = false, Length = 256)]
        public virtual string Password { get; set; }

        /// <summary> 超时时间（秒） </summary>
        [Property(Name = "Timeout", Column = "timeout", Type = "int", NotNull = false)]
        public virtual int Timeout { get; set; }

        /// <summary>使用 ssl 安全连接</summary>
        [Property(Name = "UseSsl", Column = "use_ssl", Type = "bool", NotNull = true)]
        public virtual bool UseSsl { get; set; }

        /// <summary>是否删除</summary>
        [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
        public virtual bool IsDeleted { get; set; }
    }

}
