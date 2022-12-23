using NUnit.Framework;
using Beginor.GisHub.DynamicSql;

using static NUnit.Framework.Assert;
using static System.Console;

namespace Beginor.GisHub.Test.DynamicSql;

public class ParameterConverterTest : BaseTest<ParameterConverterFactory> {

    [Test]
    public void _00_CanResolveTarget() {
        NotNull(Target);
    }

    [Test]
    public void _01_CanGetConverter() {
        var converter = Target.GetParameterConverter("int[]");
        NotNull(converter);
        WriteLine(converter);
    }

}
