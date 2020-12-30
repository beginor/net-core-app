using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>切片地图</summary>
    [Class(Schema = "public", Table = "tilemaps", Where = "is_deleted = false")]
    public partial class TileMapEntity : BaseEntity<long> {

        /// <summary>切片地图id</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>切片地图名称</summary>
        [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 32)]
        public virtual string Name { get; set; }

        /// <summary>缓存目录</summary>
        [Property(Name = "CacheDirectory", Column = "cache_directory", Type = "string", NotNull = true, Length = 512)]
        public virtual string CacheDirectory { get; set; }

        /// <summary>切片信息路径</summary>
        [Property(Name = "MapTileInfoPath", Column = "map_tile_info_path", Type = "string", NotNull = true, Length = 512)]
        public virtual string MapTileInfoPath { get; set; }

        /// <summary>内容类型</summary>
        [Property(Name = "ContentType", Column = "content_type", Type = "string", NotNull = true, Length = 64)]
        public virtual string ContentType { get; set; }

        /// <summary>是否为紧凑格式</summary>
        [Property(Name = "IsBundled", Column = "is_bundled", Type = "bool", NotNull = true)]
        public virtual bool IsBundled { get; set; }

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
