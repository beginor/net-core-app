using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class DoubleArrayParameterConverter : IParameterConverter {

    private ILogger<DoubleArrayParameterConverter> logger;

    public string ParameterType => "double[]";

    public DoubleArrayParameterConverter(ILogger<DoubleArrayParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var result = new List<double>();
        foreach (var str in arr) {
            if (double.TryParse(str, out var value)) {
                result.Add(value);
            }
        }
        return result;
    }

}