using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class IntParameterConverter : IParameterConverter {

    private ILogger<IntParameterConverter> logger;

    public string ParameterType => "int";

    public IntParameterConverter(ILogger<IntParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        if (int.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}
