namespace Beginor.GisHub.DynamicSql.ParameterConverters {

    public interface IParameterConverter {

        string ParameterType { get; }

        object ConvertParameter(string parameterValue);

    }

}
