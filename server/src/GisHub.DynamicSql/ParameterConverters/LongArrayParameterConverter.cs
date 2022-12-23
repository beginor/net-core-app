using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class LongArrayParameterConverter : IParameterConverter {

    private ILogger<LongArrayParameterConverter> logger;

    public string ParameterType => "long[]";

    public LongArrayParameterConverter(ILogger<LongArrayParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var result = new List<long>();
        foreach (var str in arr) {
            if (long.TryParse(str, out var value)) {
                result.Add(value);
            }
        }
        return result;
    }

}
