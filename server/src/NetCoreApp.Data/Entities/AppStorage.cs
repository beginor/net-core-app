using System;
using Beginor.AppFx.Core;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Data.Entities; 

/// <summary>应用存储</summary>
[Class(Schema = "public", Table = "app_storages")]
public partial class AppStorage : BaseEntity<long> {

    /// <summary>存储id</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>存储别名</summary>
    [Property(Name = "AliasName", Column = "alias_name", Type = "string", NotNull = true, Length = 32)]
    public virtual string AliasName { get; set; }

    /// <summary>存储根路径</summary>
    [Property(Name = "RootFolder", Column = "root_folder", Type = "string", NotNull = true, Length = 128)]
    public virtual string RootFolder { get; set; }

    /// <summary>是否只读</summary>
    [Property(Name = "Readonly", Column = "readonly", Type = "bool", NotNull = true)]
    public virtual bool Readonly { get; set; }

    /// <summary>可访问此存储的角色</summary>
    [Property(Name = "Roles", Column = "roles", TypeType = typeof(StringArrayType), NotNull = false)]
    public virtual string[] Roles { get; set; }
}

public class AppStorageCacheItem {
    public long Id { get; set; }
    public string AliasName { get; set; }
    public string RootFolder { get; set; }
    public bool Readonly { get; set; }
    public string[] Roles { get; set; }
}