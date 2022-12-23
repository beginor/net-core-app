using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class FloatParameterConverter : IParameterConverter {

    private ILogger<FloatParameterConverter> logger;

    public string ParameterType => "float";

    public FloatParameterConverter(ILogger<FloatParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        if (float.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}
