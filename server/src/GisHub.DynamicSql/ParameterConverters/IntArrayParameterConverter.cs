using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class IntArrayParameterConverter : IParameterConverter {

    private ILogger<IntArrayParameterConverter> logger;

    public string ParameterType => "int[]";

    public IntArrayParameterConverter(ILogger<IntArrayParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var result = new List<int>();
        foreach (var str in arr) {
            if (int.TryParse(str, out var value)) {
                result.Add(value);
            }
        }
        return result;
    }

}