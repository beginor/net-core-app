using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>矢量切片包</summary>
    [Class(Schema = "public", Table = "vectortiles", Where = "is_deleted = false")]
    public partial class VectorTileEntity : BaseEntity<long> {

        /// <summary>矢量切片包ID</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>矢量切片包名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 32)]
        public virtual string Name { get; set; }

        /// <summary>矢量切片包目录</summary>
        [Property(Name = "Directory", Column = "directory", Type = "string", NotNull = true, Length = 512)]
        public virtual string Directory { get; set; }

        /// <summary>最小缩放级别</summary>
        [Property(Name = "MinZoom", Column = "min_zoom", Type = "short", NotNull = false)]
        public virtual short? MinZoom { get; set; }

        /// <summary>最大缩放级别</summary>
        [Property(Name = "MaxZoom", Column = "max_zoom", Type = "short", NotNull = false)]
        public virtual short? MaxZoom { get; set; }

        [Property(Name = "DefaultStyle", Column = "default_style", Type = "string", Length = 32, NotNull = false)]
        public virtual string DefaultStyle { get; set; }

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
