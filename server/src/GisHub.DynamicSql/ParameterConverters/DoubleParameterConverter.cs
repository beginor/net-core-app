using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class DoubleParameterConverter : IParameterConverter {

    private ILogger<DoubleParameterConverter> logger;

    public string ParameterType => "double";

    public DoubleParameterConverter(ILogger<DoubleParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        if (double.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}