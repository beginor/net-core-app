using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class BoolArrayParameterConverter : IParameterConverter {

    private ILogger<BoolArrayParameterConverter> logger;

    public string ParameterType => "bool[]";

    public BoolArrayParameterConverter(ILogger<BoolArrayParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var result = new List<bool>();
        foreach (var str in arr) {
            if (bool.TryParse(str, out var value)) {
                result.Add(value);
            }
        }
        return result;
    }

}