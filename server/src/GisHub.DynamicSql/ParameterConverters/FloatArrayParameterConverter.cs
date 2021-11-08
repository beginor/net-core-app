using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters {

    public class FloatArrayParameterConverter : IParameterConverter {

        private ILogger<FloatArrayParameterConverter> logger;

        public string ParameterType => "float[]";

        public FloatArrayParameterConverter(ILogger<FloatArrayParameterConverter> logger) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public object ConvertParameter(string parameterValue) {
            var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var result = new List<float>();
            foreach (var str in arr) {
                if (float.TryParse(str, out var value)) {
                    result.Add(value);
                }
            }
            return result;
        }

    }

}
