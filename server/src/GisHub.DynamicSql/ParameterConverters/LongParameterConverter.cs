using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters;

public class LongParameterConverter : IParameterConverter {

    private ILogger<LongParameterConverter> logger;

    public string ParameterType => "long";

    public LongParameterConverter(ILogger<LongParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object? ConvertParameter(string parameterValue) {
        if (long.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}
