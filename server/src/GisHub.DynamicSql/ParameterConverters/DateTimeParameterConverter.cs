using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class DateTimeParameterConverter : IParameterConverter {

    private ILogger<DateTimeParameterConverter> logger;

    public string ParameterType => "datetime";

    public DateTimeParameterConverter(ILogger<DateTimeParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        if (DateTime.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}
