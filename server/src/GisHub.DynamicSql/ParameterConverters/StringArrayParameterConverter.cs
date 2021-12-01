using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters {

    public class StringArrayParameterConverter : IParameterConverter {

        private ILogger<StringArrayParameterConverter> logger;

        public string ParameterType => "string[]";

        public StringArrayParameterConverter(ILogger<StringArrayParameterConverter> logger) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public object ConvertParameter(string parameterValue) {
            var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            foreach (var str in arr) {
                result.Add(str);
            }
            return result;
        }

    }

}
