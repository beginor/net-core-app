using System.Data;
using System.Text.Json;
using Dapper;

namespace Beginor.GisHub.Common {

    public class JsonTypedHandler<T> : SqlMapper.TypeHandler<T> {

        public override T Parse(object value) {
            return (value == null) ? default(T)
                : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, T value) {
            parameter.Value = value == null ? null
                : JsonSerializer.Serialize(value);
        }

    }
}
