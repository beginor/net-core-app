using System;
using System.Collections.Generic;
using Beginor.GisHub.DynamicSql.ParameterConverters;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql;

public class ParameterConverterFactory {

    private Dictionary<string, IParameterConverter> converters;
    private ILogger<ParameterConverterFactory> logger;

    public ParameterConverterFactory(ILoggerFactory loggerFactory) {
        logger = loggerFactory.CreateLogger<ParameterConverterFactory>();
        converters = new Dictionary<string, IParameterConverter>(StringComparer.OrdinalIgnoreCase);
        IParameterConverter converter = new BoolArrayParameterConverter(loggerFactory.CreateLogger<BoolArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new BoolParameterConverter(loggerFactory.CreateLogger<BoolParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new DateTimeArrayParameterConverter(loggerFactory.CreateLogger<DateTimeArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new DateTimeParameterConverter(loggerFactory.CreateLogger<DateTimeParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new DoubleArrayParameterConverter(loggerFactory.CreateLogger<DoubleArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new DoubleParameterConverter(loggerFactory.CreateLogger<DoubleParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new FloatArrayParameterConverter(loggerFactory.CreateLogger<FloatArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new FloatParameterConverter(loggerFactory.CreateLogger<FloatParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new IntArrayParameterConverter(loggerFactory.CreateLogger<IntArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new IntParameterConverter(loggerFactory.CreateLogger<IntParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new LongArrayParameterConverter(loggerFactory.CreateLogger<LongArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new LongParameterConverter(loggerFactory.CreateLogger<LongParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new StringArrayParameterConverter(loggerFactory.CreateLogger<StringArrayParameterConverter>());
        converters.Add(converter.ParameterType, converter);
        converter = new StringParameterConverter(loggerFactory.CreateLogger<StringParameterConverter>());
        converters.Add(converter.ParameterType, converter);
    }

    public IParameterConverter GetParameterConverter(string parameterType) {
        if (converters.ContainsKey(parameterType))
            return converters[parameterType];
        else {
            logger.LogWarning($"Parameter converter for {parameterType} is not registered, use string converter instead!");
            return converters["string"];
        }
    }
}
