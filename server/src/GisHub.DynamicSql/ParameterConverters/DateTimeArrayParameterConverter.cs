using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class DateTimeArrayParameterConverter : IParameterConverter {

    private ILogger<DateTimeArrayParameterConverter> logger;

    public string ParameterType => "datetime[]";

    public DateTimeArrayParameterConverter(ILogger<DateTimeArrayParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        var arr = parameterValue.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var result = new List<DateTime>();
        foreach (var str in arr) {
            if (DateTime.TryParse(str, out var value)) {
                result.Add(value);
            }
        }
        return result;
    }

}
