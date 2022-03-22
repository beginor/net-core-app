using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class BoolParameterConverter : IParameterConverter {

    private ILogger<BoolParameterConverter> logger;

    public string ParameterType => "bool";

    public BoolParameterConverter(ILogger<BoolParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        if (bool.TryParse(parameterValue, out var value)) {
            return value;
        }
        logger.LogError($"Can not convert {parameterValue} to {ParameterType} , return value is null.");
        return null;
    }

}