using System;
using System.Text.Json;
using Beginor.AppFx.Core;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.Attributes;

namespace Beginor.NetCoreApp.Data.Entities {

    /// <summary>json 数据</summary>
    [Class(Schema = "public", Table = "app_json_data")]
    public partial class AppJsonData : BaseEntity<long> {

        /// <summary>json 数据id</summary>
        [Id(Name = "Id", Column = "id", Type = "long", Generator = "assigned")]
        public override long Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>json值</summary>
        [Property(Name = "Value", Column = "value", TypeType = typeof(JsonbType), NotNull = true)]
        public virtual JsonElement Value { get; set; }
    }

}
