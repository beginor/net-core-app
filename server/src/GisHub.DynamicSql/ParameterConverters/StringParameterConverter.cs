using System;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.ParameterConverters; 

public class StringParameterConverter : IParameterConverter {

    private ILogger<StringParameterConverter> logger;

    public string ParameterType => "string";

    public StringParameterConverter(ILogger<StringParameterConverter> logger) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public object ConvertParameter(string parameterValue) {
        return parameterValue;
    }

}