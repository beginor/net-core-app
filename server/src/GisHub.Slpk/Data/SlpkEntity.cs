using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.Slpk.Data {

    /// <summary>slpk 航拍模型</summary>
    [Class(Schema = "public", Table = "slpks", Where = "is_deleted = false")]
    public partial class SlpkEntity : BaseEntity<long> {

        /// <summary>航拍模型ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>航拍模型目录</summary>
        [Property(Name = "Directory", Column = "directory", Type = "string", NotNull = true, Length = 512)]
        public virtual string Directory { get; set; }

        /// <summary>模型经度</summary>
        [Property(Name = "Longitude", Column = "longitude", Type = "double", NotNull = true)]
        public virtual double Longitude { get; set; }

        /// <summary>模型纬度</summary>
        [Property(Name = "Latitude", Column = "latitude", Type = "double", NotNull = true)]
        public virtual double Latitude { get; set; }

        /// <summary>模型海拔高度</summary>
        [Property(Name = "Elevation", Column = "elevation", Type = "double", NotNull = true)]
        public virtual double Elevation { get; set; }

        /// <summary>标签/别名</summary>
        [Property(Name = "Tags", Column = "tags", Type = "string[]", NotNull = false)]
        public virtual string[] Tags { get; set; }

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
